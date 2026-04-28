using FastEndpoints;
using FastEndpoints.Swagger;
using Airbnb.BookingService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<BookingDbContext>("bookdb");

builder.AddKafkaConsumer<string, string>("kafka", options =>
{
    options.Config.GroupId = "booking-service-group";
});

builder.Services.AddHostedService<PaymentConsumer>();

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
