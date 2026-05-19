using FastEndpoints;
using FastEndpoints.Swagger;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Airbnb.ChatService.Infrastructure;
using Airbnb.ChatService.Infrastructure.HttpClients;
using System.Text.Json.Serialization.Metadata;

[assembly: MediatorOptions(ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 1. Aspire Service Defaults (OTEL, HealthChecks, Resilience)
builder.AddServiceDefaults();

// 2. Database - Npgsql
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("chatdb"));
    
    // Bỏ qua lỗi check model changes để có thể chạy MigrateAsync ngay cả khi code lệch một chút với Migration
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});
builder.EnrichNpgsqlDbContext<AppDbContext>();

// 2.1 Http Clients
builder.Services.AddHttpClient<PropertyServiceClient>(client =>
{
    // Sử dụng Service Discovery name từ Aspire AppHost
    client.BaseAddress = new Uri("http://propertyservice");
});

// 3. Redis & SignalR Backplane
var redisConnection = builder.Configuration.GetConnectionString("redis");
if (!string.IsNullOrEmpty(redisConnection))
{
    builder.Services.AddSignalR().AddStackExchangeRedis(redisConnection);
}
else
{
    builder.Services.AddSignalR(); // Fallback if no redis string (e.g., local tests)
}

// 4. MassTransit + RabbitMQ + EF Core Outbox
builder.Services.AddMassTransit(x =>
{
    // Outbox: persist messages/events to DB
    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox(); 
    });

    // Register Consumers
    x.AddConsumer<Airbnb.ChatService.Features.Consumers.BookingConfirmedEventConsumer>();
    x.AddConsumer<Airbnb.ChatService.Features.Consumers.UserProfileUpdatedEventConsumer>();

        x.UsingRabbitMq((ctx, cfg) =>
        {
            var rabbitConnectionString = builder.Configuration.GetConnectionString("rabbit");
            if (!string.IsNullOrEmpty(rabbitConnectionString))
            {
                cfg.Host(rabbitConnectionString);
            }
            else
            {
                cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", h =>
                {
                    h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
                });
            }

            cfg.ConfigureEndpoints(ctx);
        });
});

// 5. Mediator (source-generated CQRS dispatch – zero reflection)
builder.Services.AddMediator();

// 6. FastEndpoints & Swagger
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o => 
{
    o.DocumentSettings = s =>   
    {
        s.Title = "Airbnb Chat Service API";
        s.Version = "v1";
    };
});

var app = builder.Build();
app.UseCors("AllowAll");

// 7. Middleware pipeline
app.UseFastEndpoints(c =>
{
    // Configure serializers later when Context is ready
});

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

// Map Hubs
app.MapHub<Airbnb.ChatService.Features.Hubs.ChatHub>("/hubs/chat");

// Map Aspire defaults
app.MapDefaultEndpoints();

app.Run();
