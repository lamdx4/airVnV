using FastEndpoints;
// Force dotnet watch full rebuild and process restart to refresh route discovery metadata cache
using FastEndpoints.Swagger;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;
using Airbnb.Infrastructure.Media;

[assembly: MediatorOptions(ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

// 1. Aspire Service Defaults (OTEL, HealthChecks, Resilience)
builder.AddServiceDefaults();

// HTTP Clients
builder.Services.AddHttpClient<Airbnb.PropertyService.Infrastructure.HttpClients.BookingServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://bookingservice");
});

// 2. Database - Npgsql
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("propdb"));
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});
builder.EnrichNpgsqlDbContext<AppDbContext>();

// 2.1 Media Services (Cloudinary)
builder.Services.AddMediaServices(builder.Configuration);

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

// Bridge layer: Domain Events → MassTransit
builder.Services.AddScoped<DomainEventPublisher>();

// 4. Mediator (source-generated CQRS dispatch – zero reflection)
builder.Services.AddMediator();
builder.Services.AddAuthorization();

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

// 5. Middleware pipeline
app.UseAuthorization();
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

// 6. DB Migration & Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();
