using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;
using MassTransit;
using FastEndpoints;
using FastEndpoints.Swagger;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.SharedKernel.Infrastructure;
using Airbnb.PaymentService.Infrastructure.Messaging;
using Mediator;

[assembly: MediatorOptions(ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// Database registration
builder.Services.AddDbContext<PaymentDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("paydb"));
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});
builder.EnrichNpgsqlDbContext<PaymentDbContext>();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<Airbnb.PaymentService.Infrastructure.HttpClients.BookingServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://bookingservice");
});

builder.Services.AddHttpClient<Airbnb.PaymentService.Infrastructure.HttpClients.PropertyServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://propertyservice");
});

builder.Services.AddHttpClient<Airbnb.PaymentService.Infrastructure.HttpClients.UserServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://userservice");
    client.Timeout = TimeSpan.FromSeconds(2); // fail fast if UserService unavailable
});

builder.Services.AddScoped<Airbnb.PaymentService.Infrastructure.PaymentGateways.IPaymentProvider, Airbnb.PaymentService.Infrastructure.PaymentGateways.VnpayProvider>();
builder.Services.AddScoped<Airbnb.PaymentService.Infrastructure.PaymentGateways.PaymentProviderResolver>();

builder.Services.AddMediator();

// Event Architecture
builder.Services.AddScoped<IIntegrationEventMapper, PaymentIntegrationEventMapper>();
builder.Services.AddScoped<IIntegrationEventBridge, PaymentIntegrationEventBridge>();
builder.Services.AddScoped<IDomainEventPolicyExecutor, PaymentDomainEventPolicyExecutor>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<Airbnb.PaymentService.Infrastructure.Messaging.InitiatePaymentCommandConsumer>();
    x.AddConsumer<Airbnb.PaymentService.Features.Consumers.PaymentSucceededLedgerConsumer>();
    x.AddConsumer<Airbnb.PaymentService.Features.Consumers.RefundPaymentCommandConsumer>();

    x.AddEntityFrameworkOutbox<PaymentDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("rabbit");
        if (!string.IsNullOrEmpty(connectionString))
        {
            cfg.Host(connectionString);
        }
        else
        {
            cfg.Host("localhost", "/", h => {
                h.Username("guest");
                h.Password("guest");
            });
        }
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
        PaymentJsonContext.Default, new DefaultJsonTypeInfoResolver());
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthorization();

app.UseFastEndpoints(c => {
    c.Serializer.Options.TypeInfoResolver = JsonTypeInfoResolver.Combine(
        PaymentJsonContext.Default, new DefaultJsonTypeInfoResolver());
});

if (app.Environment.IsDevelopment())
    app.UseSwaggerGen();

app.MapDefaultEndpoints();

// DB Migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PaymentDbContext>();
        await context.Database.MigrateAsync();

        // Seed default PlatformSettings singleton if missing.
        if (!await context.PlatformSettings.AnyAsync())
        {
            context.PlatformSettings.Add(Airbnb.PaymentService.Domain.PlatformSetting.CreateDefault());
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();
