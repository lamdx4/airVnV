#pragma warning disable ASPIREMCP001, ASPIREPOSTGRES001 // Experimental MCP server & Postgres MCP APIs
using Airbnb.AppHost.Extensions;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Container Registry (GHCR) – dùng cho Aspire 9 native publisher
// Format: ghcr.io/lamdx4/airvnv/<servicename>
if (!builder.Environment.IsDevelopment())
{
    builder.AddContainerRegistry("ghcr", "ghcr.io", "lamdx4/airvnv");
}

// Enable native docker-compose publisher
builder.AddDockerComposeEnvironment("env");

// 1. Data Infrastructure (Postgres, Kafka, RabbitMQ, ElasticSearch, Redis, Debezium)
var infrastructure = builder.AddInfrastructure();

// 2. Microservices (VSA Architecture)
var microservices = builder.AddMicroservices(infrastructure);

// 3. API Gateway (YARP)
var gateway = builder.AddGateway(microservices);

// 4. Frontends (React Web App & Next.js Admin Panel)
// Tạm thời tắt để tiết kiệm tài nguyên VPS (Đã deploy lên Vercel)
// builder.AddFrontends(gateway);

// 5. Observability Stack (Grafana, Loki, Tempo, Prometheus, OTel Collector)
builder.AddMonitoring();

builder.Build().Run();
