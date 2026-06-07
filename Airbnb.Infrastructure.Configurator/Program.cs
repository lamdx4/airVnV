using Airbnb.Infrastructure.Configurator;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Sử dụng ServiceDefaults chung cho OTEL/Resilience
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

// HttpClient để gọi tới Debezium API
// Note: Worker already has its own retry loop (5s delay). Skip Polly resilience
// to avoid noisy "Standard-Retry" logs while Debezium is warming up.
builder.Services.AddHttpClient("DebeziumClient", client => {
    // Aspire sẽ tự động inject URL của Debezium vào biến môi trường
    client.BaseAddress = new Uri(builder.Configuration["services:debezium:http:0"] ?? "http://localhost:8083");
    client.Timeout = TimeSpan.FromSeconds(10);
}).RemoveAllLoggers();

var host = builder.Build();
host.Run();
