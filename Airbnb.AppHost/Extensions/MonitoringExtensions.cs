using Microsoft.Extensions.Hosting;
using System.IO;

namespace Airbnb.AppHost.Extensions;

public static class MonitoringExtensions
{
    public static IDistributedApplicationBuilder AddMonitoring(this IDistributedApplicationBuilder builder)
    {
        var monitoringPath = Path.GetFullPath(Path.Combine("..", "infras", "monitoring"));
        var isDev = builder.Environment.IsDevelopment();

        var otelCollector = builder.AddContainer("otel-collector", "otel/opentelemetry-collector-contrib", "0.128.0")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithBindMount(Path.Combine(monitoringPath, "otel-collector-config.yaml"), "/etc/otelcol-contrib/config.yaml", isReadOnly: true)
            .WithEndpoint("grpc", e => {
                if (isDev) e.Port = 4317;
                e.TargetPort = 4317;
                e.UriScheme = "tcp";
            })
            .WithEndpoint("http", e => {
                if (isDev) e.Port = 4318;
                e.TargetPort = 4318;
                e.UriScheme = "http";
            })
            .WithEndpoint("metrics", e => {
                if (isDev) e.Port = 8889;
                e.TargetPort = 8889;
                e.UriScheme = "http";
            })
            .WithEnvironment("GOGC", "50")          // Aggressive GC — keeps memory low
            .WithEnvironment("GOMAXPROCS", "1");    // Single-thread — saves CPU+RAM

        var loki = builder.AddContainer("loki", "grafana/loki", "3.5.0")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithBindMount(Path.Combine(monitoringPath, "loki-config.yaml"), "/etc/loki/local-config.yaml", isReadOnly: true)
            .WithEndpoint("http", e => {
                if (isDev) e.Port = 3100;
                e.TargetPort = 3100;
                e.UriScheme = "http";
            })
            .WithEnvironment("GOGC", "50")
            .WithEnvironment("GOMAXPROCS", "1")
            .WithArgs("-config.file=/etc/loki/local-config.yaml");

        var tempo = builder.AddContainer("tempo", "grafana/tempo", "2.7.2")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithBindMount(Path.Combine(monitoringPath, "tempo-config.yaml"), "/etc/tempo/tempo-config.yaml", isReadOnly: true)
            .WithEndpoint("http", e => {
                if (isDev) e.Port = 3200;
                e.TargetPort = 3200;
                e.UriScheme = "http";
            })
            .WithEnvironment("GOGC", "50")
            .WithEnvironment("GOMAXPROCS", "1")
            .WithArgs("-config.file=/etc/tempo/tempo-config.yaml");

        var prometheus = builder.AddContainer("prometheus", "prom/prometheus", "v3.4.0")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithBindMount(Path.Combine(monitoringPath, "prometheus.yml"), "/etc/prometheus/prometheus.yml", isReadOnly: true)
            .WithEndpoint("http", e => {
                if (isDev) e.Port = 9090;
                e.TargetPort = 9090;
                e.UriScheme = "http";
            })
            .WithArgs(
                "--config.file=/etc/prometheus/prometheus.yml",
                "--storage.tsdb.retention.time=3d",
                "--storage.tsdb.retention.size=400MB",
                "--web.enable-remote-write-receiver",   // Accept remote-write from OTel Collector
                "--log.level=warn"
            )
            .WaitFor(otelCollector);

        var grafana = builder.AddContainer("grafana", "grafana/grafana-oss", "12.0.1")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithBindMount(Path.Combine(monitoringPath, "grafana", "provisioning"), "/etc/grafana/provisioning", isReadOnly: true)
            .WithEndpoint("http", e => {
                if (isDev) e.Port = 3000;
                e.TargetPort = 3000;
                e.UriScheme = "http";
            })
            .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "true")
            .WithEnvironment("GF_AUTH_ANONYMOUS_ORG_ROLE", "Admin")  // No login needed in dev
            .WithEnvironment("GF_AUTH_DISABLE_LOGIN_FORM", "true")
            .WithEnvironment("GF_ANALYTICS_REPORTING_ENABLED", "false")
            .WithEnvironment("GF_ANALYTICS_CHECK_FOR_UPDATES", "false")
            .WithEnvironment("GF_LOG_LEVEL", "warn")
            .WaitFor(loki)
            .WaitFor(tempo)
            .WaitFor(prometheus);

        return builder;
    }
}
