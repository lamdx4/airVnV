using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Registry;

namespace Airbnb.Infrastructure.Configurator;

public class Worker(
    ILogger<Worker> logger, 
    IHttpClientFactory httpClientFactory,
    ResiliencePipelineProvider<string> pipelineProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Configurator Worker starting...");
        var pipeline = pipelineProvider.GetPipeline("idempotent-pipeline");
        
        try 
        {
            await pipeline.ExecuteAsync(async ct => {
                var client = httpClientFactory.CreateClient("DebeziumClient");
                var response = await client.GetAsync("/", ct);
                if (!response.IsSuccessStatusCode) throw new Exception("Debezium not ready.");

                // Cấu hình danh sách Connector cho từng Database
                await ConfigureConnector(client, "property-connector", "propertydb", "public.Properties", ct);
                await ConfigureConnector(client, "payment-outbox-connector", "paydb", "public.OutboxEvents", ct);
                
            }, stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Configurator Worker failed.");
        }
    }

    private async Task ConfigureConnector(HttpClient client, string name, string dbName, string tableList, CancellationToken ct)
    {
        logger.LogInformation("Configuring connector {name} for DB {db}...", name, dbName);
        
        var config = new
        {
            name = name,
            config = new
            {
                connector_class = "io.debezium.connector.postgresql.PostgresConnector",
                tasks_max = "1",
                database_hostname = "postgres", 
                database_port = "5432",
                database_user = "postgres",
                database_password = "password",
                database_dbname = dbName,
                topic_prefix = "airbnb", // Dùng chung prefix
                plugin_name = "pgoutput",
                table_include_list = tableList,
                key_converter = "org.apache.kafka.connect.json.JsonConverter",
                value_converter = "org.apache.kafka.connect.json.JsonConverter",
                // Staff-level: Kích hoạt SMT để xử lý CDC tốt hơn nếu cần
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
