using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;
using MassTransit;
using FastEndpoints;
using FastEndpoints.Swagger;
using Airbnb.PaymentService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<PaymentDbContext>("paydb");

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

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services.AddMassTransit(x =>
{
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

app.UseFastEndpoints(c => {
    c.Serializer.Options.TypeInfoResolver = PaymentJsonContext.Default;
});

if (app.Environment.IsDevelopment())
    app.UseSwaggerGen();

app.MapDefaultEndpoints();
app.Run();
