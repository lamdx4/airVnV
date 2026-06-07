using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Airbnb.Infrastructure.Configurator;

public class Worker(
    ILogger<Worker> logger, 
    IHttpClientFactory httpClientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Debezium Configurator Worker starting...");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try 
            {
                var client = httpClientFactory.CreateClient("DebeziumClient");
                logger.LogInformation("Checking Debezium Connect readiness at {Url}...", client.BaseAddress);
                
                var response = await client.GetAsync("/", stoppingToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Debezium returned status code: {response.StatusCode}");
                }

                logger.LogInformation("Debezium Connect is active! Configuring connectors...");

                // Cấu hình danh sách Connector cho từng Database
                await ConfigureConnector(client, "property-connector-v2", "propdb", "public.properties,public.property_images", stoppingToken);
                
                logger.LogInformation("Debezium Configurator completed successfully.");
                break; // Exit loop on success
            }
            catch (Exception ex)
            {
                logger.LogWarning("Debezium Connect is not ready yet: {Message}. Retrying in 5 seconds...", ex.Message);
                try
                {
                    await Task.Delay(5000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private async Task ConfigureConnector(HttpClient client, string name, string dbName, string tableList, CancellationToken ct)
    {
        logger.LogInformation("Configuring connector {name} for DB {db}...", name, dbName);
        
        var config = new
        {
            name = name,
            config = new Dictionary<string, string>
            {
                { "connector.class", "io.debezium.connector.postgresql.PostgresConnector" },
                { "tasks.max", "1" },
                { "database.hostname", "postgres" },
                { "database.port", "5432" },
                { "database.user", "postgres" },
                { "database.password", Environment.GetEnvironmentVariable("PG_PASSWORD") ?? "password" },
                { "database.dbname", dbName },
                { "topic.prefix", "airbnb" },
                { "plugin.name", "pgoutput" },
                { "table.include.list", tableList },
                { "key.converter", "org.apache.kafka.connect.json.JsonConverter" },
                { "value.converter", "org.apache.kafka.connect.json.JsonConverter" },
                { "decimal.handling.mode", "double" }
            }
        };

        var json = JsonSerializer.Serialize(config);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var postResponse = await client.PostAsync("/connectors", content, ct);

        if (postResponse.IsSuccessStatusCode || postResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            logger.LogInformation("Connector {name} ready.", name);
        }
        else
        {
            var error = await postResponse.Content.ReadAsStringAsync(ct);
            throw new Exception($"Failed to config {name}: {error}");
        }
    }
}
