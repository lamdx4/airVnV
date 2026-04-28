using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ServiceDiscovery;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.CircuitBreaker;
using Microsoft.Extensions.Http.Resilience;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();
        builder.AddDefaultHealthChecks();
        builder.Services.AddServiceDiscovery();

        // 2. Polly V8: Chống "Tự sát" (Cascading Failures)
        builder.AddServiceResilience();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddServiceDiscovery();
            // Default resilience if needed, we can also use named pipelines
            http.AddStandardResilienceHandler();
        });

        return builder;
    }

    public static void AddServiceResilience(this IHostApplicationBuilder builder)
    {
        // 2.1. Phân loại Chiến lược theo đặc tính Endpoint
        
        // Cấu hình cho các thao tác Idempotent (Đọc/GET) - Retry thoải mái
        builder.Services.AddResiliencePipeline("idempotent-pipeline", pipelineBuilder => {
            pipelineBuilder.AddRetry(new RetryStrategyOptions {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true
            }).AddTimeout(TimeSpan.FromSeconds(5));
        });

        // Cấu hình cho thao tác Non-idempotent (Ghi/Thanh toán) - Không Retry, chỉ Timeout & Circuit Breaker
        builder.Services.AddResiliencePipeline("non-idempotent-pipeline", pipelineBuilder => {
            pipelineBuilder.AddCircuitBreaker(new CircuitBreakerStrategyOptions {
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                SamplingDuration = TimeSpan.FromSeconds(30)
            }).AddTimeout(TimeSpan.FromSeconds(15));
            
            // 4.1. Bổ sung Concurrency Limiter (Bulkhead)
            pipelineBuilder.AddConcurrencyLimiter(100, 50);
        });
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
                
                // Resilience metrics are collected if Microsoft.Extensions.Resilience is used
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation(tracing =>
                        tracing.Filter = context =>
                            !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                            && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                    )
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        return builder;
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapHealthChecks(HealthEndpointPath);
            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}
