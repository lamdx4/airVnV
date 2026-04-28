using FastEndpoints;
using FastEndpoints.Swagger;
using Airbnb.PaymentService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<PaymentDbContext>("paydb");
builder.AddKafkaProducer<string, string>("kafka");

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
