using Microsoft.Extensions.Hosting;

namespace Airbnb.AppHost.Models;

public record AppInfrastructure(
    IResourceBuilder<PostgresServerResource> Postgres,
    IResourceBuilder<PostgresDatabaseResource> UserDb,
    IResourceBuilder<PostgresDatabaseResource> PropDb,
    IResourceBuilder<PostgresDatabaseResource> BookDb,
    IResourceBuilder<PostgresDatabaseResource> PayDb,
    IResourceBuilder<PostgresDatabaseResource> ChatDb,
    IResourceBuilder<KafkaServerResource> Kafka,
    IResourceBuilder<RabbitMQServerResource> RabbitMq,
    IResourceBuilder<ElasticsearchResource> Elasticsearch,
    IResourceBuilder<RedisResource> Redis,
    IResourceBuilder<ContainerResource> Debezium,
    IResourceBuilder<ContainerResource> Coturn,
    IResourceBuilder<ParameterResource> RootDomain
);
