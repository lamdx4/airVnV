using FastEndpoints;
using FastEndpoints.Swagger;
using Airbnb.PropertyService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

// 1. Aspire Service Defaults (OTEL, HealthChecks, Resilience)
builder.AddServiceDefaults();

// 2. Database - Npgsql (Tự động lấy connection string "propdb" từ Aspire)
builder.AddNpgsqlDbContext<AppDbContext>("propdb");

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
    if (builder.Environment.IsDevelopment())
    {
        // Fail-fast: Nếu thiếu DTO trong context, app sẽ văng lỗi ngay lúc Dev
        options.SerializerOptions.TypeInfoResolver = PropertyJsonContext.Default;
    }
    else
    {
        // Production: Combine với Fallback an toàn (nhưng ưu tiên AOT context trước)
        options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
            PropertyJsonContext.Default,
            new DefaultJsonTypeInfoResolver()
        );
    }
});

var app = builder.Build();

// 5. Middleware pipeline
app.UseFastEndpoints(c =>
{
    // Đồng bộ hóa JsonSerializerOptions cho FastEndpoints
    c.Serializer.Options.TypeInfoResolver = PropertyJsonContext.Default;
});

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

// Map các endpoint mặc định của Aspire (health, discovery)
app.MapDefaultEndpoints();

app.Run();
