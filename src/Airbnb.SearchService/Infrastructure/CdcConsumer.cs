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

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                if (result == null) continue;

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
                        BasePrice = after.GetProperty("pricing_base_price").GetDecimal(),
                        AverageRating = after.GetProperty("AverageRating").GetDecimal(),
                        ReviewCount = after.GetProperty("ReviewCount").GetInt32(),
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
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing CDC message");
                await Task.Delay(5000, stoppingToken); // Backoff
            }
        }
    }
}
