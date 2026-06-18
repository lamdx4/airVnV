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
builder.AddDockerComposeEnvironment("compose")
    .WithProperties(env =>
    {
        env.DashboardEnabled = true;
    });

// 1. Infrastructure
var infrastructure = builder.AddInfrastructure();

// 2. Microservices
var microservices = builder.AddMicroservices(infrastructure);

// 3. API Gateway
builder.AddGateway(microservices);

// 4. Observability
builder.AddMonitoring();

builder.Build().Run();
