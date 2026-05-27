using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;
using MassTransit;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.SharedKernel.Infrastructure;
using Airbnb.PaymentService.Infrastructure.Messaging;
using Mediator;

[assembly: MediatorOptions(ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// JWT Authentication
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing"));
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin", "Moderator"));
});

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
    if (builder.Environment.IsDevelopment())
        options.SerializerOptions.TypeInfoResolver = PaymentJsonContext.Default;
    else
        options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
            PaymentJsonContext.Default, new DefaultJsonTypeInfoResolver());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(c => {
    c.Serializer.Options.TypeInfoResolver = PaymentJsonContext.Default;
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
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();
