using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using Airbnb.SearchService.Domain;
using System.Text.Json;

namespace Airbnb.SearchService.Infrastructure;

public class CdcConsumer(
    ILogger<CdcConsumer> logger,
    IConsumer<string, string> consumer,
    ElasticsearchClient elasticClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Topic name convention from Debezium: [topic_prefix].[schema].[table]
        // In our configurator we set topic_prefix = "airbnb"
        const string topic = "airbnb.public.Properties";
        
        consumer.Subscribe(topic);
        logger.LogInformation("Subscribed to CDC topic: {topic}", topic);

        bool isConnected = false;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                ConsumeResult<string, string>? result = null;

                if (!isConnected)
                {
                    // Lần đầu: Dùng timeout 2s để test xem Topic đã tồn tại chưa mà không bị block vĩnh viễn
                    result = consumer.Consume(TimeSpan.FromSeconds(2));
                    
                    // Nếu Consume(2s) không ném ConsumeException, nghĩa là Topic đã tồn tại!
                    logger.LogInformation("Successfully connected and listening to CDC topic '{topic}'.", topic);
                    isConnected = true; // Đánh dấu để từ vòng lặp sau sẽ dùng Blocking call native
                }
                else
                {
                    // Từ lần sau: Dùng Blocking call native để Kafka C-Core tự sleep, tiết kiệm 100% CPU
                    result = consumer.Consume(stoppingToken);
                }

                if (result == null) continue;

                await ProcessMessageAsync(result, stoppingToken);
            }
            catch (OperationCanceledException) { break; }
            catch (ConsumeException ex) when (ex.Error.Reason.Contains("Unknown topic", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("CDC topic '{topic}' is not ready yet. Waiting for Debezium to create it...", topic);
                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing CDC message");
                await Task.Delay(5000, stoppingToken); // Backoff
            }
        }
    }

    private async Task ProcessMessageAsync(ConsumeResult<string, string> result, CancellationToken stoppingToken)
    {
        logger.LogInformation("Received CDC message from Kafka: {key}", result.Message.Key);

        // Staff-level: Parse Debezium JSON (Payload contains 'before' and 'after' states)
        using var doc = JsonDocument.Parse(result.Message.Value);
        var payload = doc.RootElement.GetProperty("payload");
        
        // Op 'u' = update, 'c' = create
        var op = payload.GetProperty("op").GetString();
        
        if (op == "c" || op == "u" || op == "r") // r is for initial snapshot
        {
            var after = payload.GetProperty("after");
            var propertyDoc = new PropertyDoc
            {
                Id = Guid.Parse(after.GetProperty("Id").GetString()!),
                HostId = Guid.Parse(after.GetProperty("HostId").GetString()!),
                Title = after.GetProperty("Title").GetString()!,
                Description = after.GetProperty("Description").GetString()!,
                Slug = after.GetProperty("Slug").GetString()!,
                PropertyType = after.GetProperty("Type").GetInt32(),
                BasePrice = after.GetProperty("pricing_base_price").GetDecimal(),
                AverageRating = after.GetProperty("AverageRating").GetDecimal(),
                ReviewCount = after.GetProperty("ReviewCount").GetInt32(),
                Location = $"{after.GetProperty("Latitude").GetDouble()},{after.GetProperty("Longitude").GetDouble()}",
                Address = new AddressVO
                {
                    CountryCode = after.GetProperty("CountryCode").GetString()!,
                    Admin1Code = after.TryGetProperty("Admin1Code", out var a1) ? a1.GetString() : null,
                    Admin2Code = after.TryGetProperty("Admin2Code", out var a2) ? a2.GetString() : null,
                    DisplayAddress = after.GetProperty("DisplayAddress").GetString()!,
                    Latitude = after.GetProperty("Latitude").GetDouble(),
                    Longitude = after.GetProperty("Longitude").GetDouble()
                },
                CreatedAt = DateTime.Parse(after.GetProperty("CreatedAt").GetString()!)
            };

            // Staff-level SOP: Upsert (Idempotency)
            await elasticClient.IndexAsync(propertyDoc, i => i.Index("properties"), stoppingToken);
            logger.LogInformation("Synced property {id} to Elasticsearch", propertyDoc.Id);
        }
        else if (op == "d")
        {
            var before = payload.GetProperty("before");
            var id = Guid.Parse(before.GetProperty("Id").GetString()!);
            await elasticClient.DeleteAsync<PropertyDoc>(id, d => d.Index("properties"), stoppingToken);
            logger.LogInformation("Deleted property {id} from Elasticsearch", id);
        }
    }
}
