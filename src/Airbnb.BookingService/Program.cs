using FastEndpoints;
using FastEndpoints.Swagger;
using Airbnb.BookingService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;
using MassTransit;
using Quartz;
using Airbnb.SharedKernel.Infrastructure;
using Airbnb.BookingService.Infrastructure.Messaging;
using Mediator;

[assembly: MediatorOptions(ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// Business DbContext
builder.Services.AddDbContext<BookingDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("bookdb"));
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});
// builder.EnrichNpgsqlDbContext<BookingDbContext>();
// NOTE: Commented out — NpgsqlHealthCheck hangs on DCP proxy port
// because proxy does not forward PostgreSQL protocol → health check timeout

// Saga DbContext
builder.Services.AddDbContext<Airbnb.BookingService.Infrastructure.Saga.BookingSagaDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("bookdb"));
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});
// builder.EnrichNpgsqlDbContext<Airbnb.BookingService.Infrastructure.Saga.BookingSagaDbContext>();
// NOTE: Same reason as above


// NOTE: AddKafkaConsumer removed — no actual Kafka consumers exist in BookingService.
// All events are handled via MassTransit/RabbitMQ (MasterDataCacheInvalidationConsumer etc.)


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<Airbnb.BookingService.Features.MasterData.MasterDataCacheInvalidationConsumer>();
    x.AddConsumer<Airbnb.BookingService.Features.Consumers.PaymentSucceededConsumer>();
    x.AddConsumer<Airbnb.BookingService.Features.Consumers.PaymentFailedConsumer>();
    x.AddConsumer<Airbnb.BookingService.Features.Consumers.PaymentRefundedConsumer>();
    x.AddConsumer<Airbnb.BookingService.Features.Consumers.BookingCancelledConsumer>();

    // Saga Configuration
    x.AddSagaStateMachine<Airbnb.BookingService.Infrastructure.Saga.BookingStateMachine, Airbnb.BookingService.Infrastructure.Saga.BookingState>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<Airbnb.BookingService.Infrastructure.Saga.BookingSagaDbContext>();
            r.UsePostgres();
        });

    x.AddEntityFrameworkOutbox<BookingDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddQuartz();
    x.AddQuartzConsumers(); // BẮT BUỘC: Thêm consumer để Quartz nhận được lệnh Schedule từ Saga
    builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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

        cfg.UseMessageRetry(r => 
        {
            r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(3));
            r.Ignore<Airbnb.ServiceDefaults.Infrastructure.BusinessException>();
            r.Ignore<Airbnb.ServiceDefaults.Infrastructure.NotFoundException>();
        });

        cfg.UseMessageScheduler(new Uri("queue:quartz")); // Send schedule cmds directly to Quartz queue
        cfg.ConfigureEndpoints(context);
    });
});

// Allow HTTP to start accepting requests before MassTransit/Kafka finish connecting.
// BackgroundServiceExceptionBehavior defaults to StopHost in .NET 6+ — a single
// background service failure (e.g. Kafka consumer) would silently kill the whole host.
builder.Services.Configure<HostOptions>(opts =>
{
    opts.ServicesStartConcurrently = true;
    opts.StartupTimeout = TimeSpan.FromSeconds(60);
    opts.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
});

builder.Services.AddAuthorization();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<Airbnb.BookingService.Infrastructure.HttpClients.PropertyServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://propertyservice");
});

// Event Architecture
builder.Services.AddScoped<IIntegrationEventMapper, BookingIntegrationEventMapper>();
builder.Services.AddScoped<IIntegrationEventBridge, BookingIntegrationEventBridge>();
builder.Services.AddScoped<IDomainEventPolicyExecutor, BookingDomainEventPolicyExecutor>();

builder.Services.AddMediator();

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    if (builder.Environment.IsDevelopment())
        options.SerializerOptions.TypeInfoResolver = BookingJsonContext.Default;
    else
        options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
            BookingJsonContext.Default, new DefaultJsonTypeInfoResolver());
});

var app = builder.Build();

app.UseMiddleware<Airbnb.ServiceDefaults.Infrastructure.ExceptionHandlingMiddleware>();
app.UseAuthorization();

app.UseFastEndpoints(c => {
    c.Serializer.Options.TypeInfoResolver = BookingJsonContext.Default;
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
        var context = services.GetRequiredService<BookingDbContext>();
        await context.Database.MigrateAsync();

        var sagaContext = services.GetRequiredService<Airbnb.BookingService.Infrastructure.Saga.BookingSagaDbContext>();
        await sagaContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();
