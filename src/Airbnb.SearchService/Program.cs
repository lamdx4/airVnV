using FastEndpoints;
using FastEndpoints.Swagger;
using Airbnb.SearchService.Infrastructure;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Aspire integration for Elasticsearch & Kafka
builder.AddElasticsearchClient("elasticsearch");
builder.AddRedisOutputCache("cache"); // Tích hợp Redis Cache của Aspire
builder.AddKafkaConsumer<string, string>("kafka", options =>
{
    options.Config.GroupId = "search-service-group";
    options.Config.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
});

builder.Services.AddHostedService<CdcConsumer>();
builder.Services.AddMediator(o => o.ServiceLifetime = ServiceLifetime.Scoped); // CQRS

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

app.UseOutputCache();

app.UseFastEndpoints(c => {
    c.Serializer.Options.TypeInfoResolver = SearchJsonContext.Default;
});

if (app.Environment.IsDevelopment())
    app.UseSwaggerGen();

app.MapDefaultEndpoints();

// Initialize Elasticsearch Index Mapping for Geo_Point
using (var scope = app.Services.CreateScope())
{
    var elastic = scope.ServiceProvider.GetRequiredService<Elastic.Clients.Elasticsearch.ElasticsearchClient>();
    
    // Check if index exists
    var existsResponse = elastic.Indices.ExistsAsync("properties").GetAwaiter().GetResult();
    if (!existsResponse.Exists)
    {
        // Create index with mapping
        elastic.Indices.CreateAsync("properties", c => c
            .Mappings(m => m
                .Properties(p => p
                    .GeoPoint("location") // Cực kỳ quan trọng: Ép kiểu geo_point
                )
            )
        ).GetAwaiter().GetResult();
    }
}

app.Run();
