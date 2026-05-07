using FastEndpoints;
using FastEndpoints.Swagger;
using Airbnb.BookingService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<BookingDbContext>("bookdb");

builder.AddKafkaConsumer<string, string>("kafka", options =>
{
    options.Config.GroupId = "booking-service-group";
});

builder.Services.AddHostedService<Airbnb.BookingService.Infrastructure.Workers.BookingTimeoutWorker>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<Airbnb.BookingService.Features.MasterData.MasterDataCacheInvalidationConsumer>();
    x.AddConsumer<Airbnb.BookingService.Features.Payments.PaymentSucceededConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("rabbitmq");
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

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<Airbnb.BookingService.Infrastructure.HttpClients.PropertyServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://propertyservice");
});

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

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

app.UseFastEndpoints(c => {
    c.Serializer.Options.TypeInfoResolver = BookingJsonContext.Default;
});

if (app.Environment.IsDevelopment())
    app.UseSwaggerGen();

app.MapDefaultEndpoints();
app.Run();
