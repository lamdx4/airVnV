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
                        Name = after.GetProperty("Name").GetString()!,
                        Description = after.GetProperty("Description").GetString()!,
                        PricePerNight = after.GetProperty("PricePerNight").GetDecimal(),
                        Address = new AddressVO
                        {
                            CountryCode = after.GetProperty("Address_CountryCode").GetString()!,
                            City = after.GetProperty("Address_City").GetString()!,
                            StateProvince = after.TryGetProperty("Address_StateProvince", out var sp) ? sp.GetString() : null,
                            Ward = after.TryGetProperty("Address_Ward", out var w) ? w.GetString() : null,
                            StreetLine1 = after.GetProperty("Address_StreetLine1").GetString()!,
                            StreetLine2 = after.TryGetProperty("Address_StreetLine2", out var s2) ? s2.GetString() : null,
                            PostalCode = after.TryGetProperty("Address_PostalCode", out var pc) ? pc.GetString() : null,
                            Latitude = after.GetProperty("Address_Latitude").GetDouble(),
                            Longitude = after.GetProperty("Address_Longitude").GetDouble()
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
