#pragma warning disable ASPIREPOSTGRES001
using Airbnb.AppHost.Models;
using Microsoft.Extensions.Hosting;
using System.IO;
using System;

namespace Airbnb.AppHost.Extensions;

public static class InfrastructureExtensions
{
    public static AppInfrastructure AddInfrastructure(this IDistributedApplicationBuilder builder)
    {
        // Container Runtime Config
        Environment.SetEnvironmentVariable("DOTNET_ASPIRE_CONTAINER_RUNTIME", "docker");

        var kafkaHeap = "-Xms512m -Xmx512m";
        var elasticHeap = "-Xms512m -Xmx512m";
        var isDev = builder.Environment.IsDevelopment();

        // 1. Data Infrastructure
        var postgres = builder.AddPostgres("postgres")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataBindMount("./data/postgres")
            .WithEnvironment("POSTGRES_INITDB_ARGS", "-c wal_level=logical")
            .WithEndpoint("tcp", e => {
                if (isDev) e.Port = 5435;
                e.TargetPort = 5432;
            });

        var userDb = postgres.AddDatabase("userdb").WithPostgresMcp();
        var propDb = postgres.AddDatabase("propdb").WithPostgresMcp();
        var bookDb = postgres.AddDatabase("bookdb").WithPostgresMcp();
        var payDb = postgres.AddDatabase("paydb").WithPostgresMcp();
        var chatDb = postgres.AddDatabase("chatdb").WithPostgresMcp();

        var kafka = builder.AddKafka("kafka")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataBindMount("./data/kafka")
            .WithKafkaUI()
            .WithEnvironment("KAFKA_HEAP_OPTS", kafkaHeap)
            .WithEndpoint("tcp", e => {
                if (isDev) e.Port = 29092;
                e.TargetPort = 9092;
            });

        // RabbitMQ – Domain Events + MassTransit Saga
        var rabbit = builder.AddRabbitMQ("rabbit")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataBindMount("./data/rabbit")
            .WithEndpoint("tcp", e => {
                if (isDev) e.Port = 5672;
                e.TargetPort = 5672;
            })
            .WithManagementPlugin(); // UI: http://localhost:15672

        var elasticsearch = builder.AddElasticsearch("elasticsearch")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataBindMount("./data/elasticsearch")
            .WithEnvironment("ES_JAVA_OPTS", elasticHeap)
            .WithEnvironment("http.cors.enabled", "true")
            .WithEnvironment("http.cors.allow-origin", "http://localhost:8080")
            .WithEnvironment("http.cors.allow-headers", "X-Requested-With,X-Auth-Token,Content-Type,Content-Length,Authorization")
            .WithEnvironment("http.cors.allow-credentials", "true")
            .WithEndpoint("http", e => {
                if (isDev) e.Port = 9200;
                e.TargetPort = 9200;
            });

        var esPassword = builder.Configuration["Parameters:elasticsearch-password"] ?? "W~0+2_CNTqd}vr9F9DSpUY";
        var elasticvueClusters = $"[{{\"name\": \"Airbnb Dev Cluster\", \"uri\": \"http://localhost:9200\", \"username\": \"elastic\", \"password\": \"{esPassword}\"}}]";

        var elasticvue = builder.AddContainer("elasticvue", "cars10/elasticvue")
            .WithEndpoint("elasticvue-ui", e => {
                if (isDev) e.Port = 8080;
                e.TargetPort = 8080;
            })
            .WithEnvironment("ELASTICVUE_CLUSTERS", elasticvueClusters)
            .WithLifetime(ContainerLifetime.Persistent);

        var redis = builder.AddRedis("redis")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataBindMount("./data/redis")
            .WithEndpoint("tcp", e => {
                if (isDev) e.Port = 6379;
                e.TargetPort = 6379;
            });

        // 2. Debezium (CDC)
        var debezium = builder.AddContainer("debezium", "docker.io/debezium/connect:2.5")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithHttpEndpoint(port: isDev ? 8083 : null, targetPort: 8083, name: "http")
            .WithEnvironment("BOOTSTRAP_SERVERS", kafka.GetEndpoint("internal"))
            .WithEnvironment("GROUP_ID", "1")
            .WithEnvironment("CONFIG_STORAGE_TOPIC", "my_connect_configs")
            .WithEnvironment("OFFSET_STORAGE_TOPIC", "my_connect_offsets")
            .WithEnvironment("STATUS_STORAGE_TOPIC", "my_connect_statuses")
            .WithEnvironment("KEY_CONVERTER", "org.apache.kafka.connect.json.JsonConverter")
            .WithEnvironment("VALUE_CONVERTER", "org.apache.kafka.connect.json.JsonConverter")
            .WithEnvironment("JAVA_OPTS", "-Xms256m -Xmx512m")
            .WithReference(postgres)
            .WithReference(kafka)
            .WaitFor(kafka);

        // 3. Worker Configurator
        builder.AddProject<Projects.Airbnb_Infrastructure_Configurator>("debezium-configurator")
            .WithReference(debezium.GetEndpoint("http"))
            .WithEnvironment("PG_PASSWORD", builder.Configuration["Parameters:postgres-password"] ?? "6t.*gWySwyQkbEr0T5rPby")
            .WaitFor(debezium);
    

        return new AppInfrastructure(
            postgres,
            userDb,
            propDb,
            bookDb,
            payDb,
            chatDb,
            kafka,
            rabbit,
            elasticsearch,
            redis,
            debezium
        );
    }
}
