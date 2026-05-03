using FastEndpoints;
using FastEndpoints.Swagger;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

// 1. Aspire Service Defaults (OTEL, HealthChecks, Resilience)
builder.AddServiceDefaults();

// 2. Database - Npgsql (Tự động lấy connection string "propdb" từ Aspire)
builder.AddNpgsqlDbContext<AppDbContext>("propdb");

// 3. MassTransit + RabbitMQ + EF Core Outbox
builder.Services.AddMassTransit(x =>
{
    // Outbox: persist messages vào DB, atomicity với domain data
    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox(); // Background service poll DB → dispatch tới RabbitMQ
    });

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

// Bridge layer: Domain Events → MassTransit
builder.Services.AddScoped<DomainEventPublisher>();

// 4. Mediator (source-generated CQRS dispatch – zero reflection)
builder.Services.AddMediator();

// 3. FastEndpoints & Swagger
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o => 
{
    o.DocumentSettings = s => 
    {
        s.Title = "Airbnb Property Service API";
        s.Version = "v1";
    };
});

// 4. Staff-level JSON Source Gen SOP (Hybrid & Fail-fast)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
        PropertyJsonContext.Default, 
        new DefaultJsonTypeInfoResolver());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// 5. Middleware pipeline
app.UseFastEndpoints(c =>
{
    c.Serializer.Options.TypeInfoResolver = JsonTypeInfoResolver.Combine(
        PropertyJsonContext.Default, 
        new DefaultJsonTypeInfoResolver());
});

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

// Map các endpoint mặc định của Aspire (health, discovery)
app.MapDefaultEndpoints();

app.Run();
