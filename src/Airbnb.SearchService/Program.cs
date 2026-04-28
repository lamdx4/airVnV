using FastEndpoints;
using FastEndpoints.Swagger;
using Airbnb.SearchService.Infrastructure;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Aspire integration for Elasticsearch & Kafka
builder.AddElasticsearchClient("elasticsearch");
builder.AddKafkaConsumer<string, string>("kafka", options =>
{
    options.Config.GroupId = "search-service-group";
    options.Config.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
});

builder.Services.AddHostedService<CdcConsumer>();

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    if (builder.Environment.IsDevelopment())
        options.SerializerOptions.TypeInfoResolver = SearchJsonContext.Default;
    else
        options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
            SearchJsonContext.Default, new DefaultJsonTypeInfoResolver());
});

var app = builder.Build();

app.UseFastEndpoints(c => {
    c.Serializer.Options.TypeInfoResolver = SearchJsonContext.Default;
});

if (app.Environment.IsDevelopment())
    app.UseSwaggerGen();

app.MapDefaultEndpoints();
app.Run();
