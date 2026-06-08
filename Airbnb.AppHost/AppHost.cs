#pragma warning disable ASPIREMCP001, ASPIREPOSTGRES001 // Experimental MCP server & Postgres MCP APIs
using Airbnb.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

// Đọc config môi trường
var containerRuntime = builder.Configuration["ContainerRuntime"] ?? "docker";
Environment.SetEnvironmentVariable("DOTNET_ASPIRE_CONTAINER_RUNTIME", containerRuntime);

var kafkaHeap = builder.Configuration["KafkaHeap"] ?? "-Xms256m -Xmx256m";
var elasticHeap = builder.Configuration["ElasticHeap"] ?? "-Xms256m -Xmx256m";

// 1. Hạ tầng Dữ liệu (Infrastructure)
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("airbnb_pg_data")
    .WithEnvironment("POSTGRES_INITDB_ARGS", "-c wal_level=logical")
    .WithEndpoint("tcp", e => {
        e.Port = 5435;
        e.TargetPort = 5432;
    });

var userDb = postgres.AddDatabase("userdb").WithPostgresMcp();
var propDb = postgres.AddDatabase("propdb").WithPostgresMcp();
var bookDb = postgres.AddDatabase("bookdb").WithPostgresMcp();
var payDb = postgres.AddDatabase("paydb").WithPostgresMcp();
var chatDb = postgres.AddDatabase("chatdb").WithPostgresMcp();

var kafka = builder.AddKafka("kafka")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("airbnb_kafka_data")
    .WithKafkaUI()
    .WithEnvironment("KAFKA_HEAP_OPTS", kafkaHeap)
    .WithEndpoint("tcp", e => {
        e.Port = 29092;
        e.TargetPort = 9092;
    });

// RabbitMQ – Domain Events + MassTransit Saga (PropertyService, BookingService, v.v.)
var rabbit = builder.AddRabbitMQ("rabbit")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("airbnb_rabbit_data")
    .WithEndpoint("tcp", e => {
        e.Port = 5672;
        e.TargetPort = 5672;
    })
    .WithManagementPlugin(); // UI: http://localhost:15672

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("airbnb_es_data")
    .WithEnvironment("ES_JAVA_OPTS", elasticHeap)
    .WithEnvironment("http.cors.enabled", "true")
    .WithEnvironment("http.cors.allow-origin", "http://localhost:8080")
    .WithEnvironment("http.cors.allow-headers", "X-Requested-With,X-Auth-Token,Content-Type,Content-Length,Authorization")
    .WithEnvironment("http.cors.allow-credentials", "true")
    .WithHttpEndpoint(port: 9200, targetPort: 9200, name: "http");

var esPassword = builder.Configuration["Parameters:elasticsearch-password"] ?? "W~0+2_CNTqd}vr9F9DSpUY";
var elasticvueClusters = $"[{{\"name\": \"Airbnb Dev Cluster\", \"uri\": \"http://localhost:9200\", \"username\": \"elastic\", \"password\": \"{esPassword}\"}}]";

var elasticvue = builder.AddContainer("elasticvue", "cars10/elasticvue")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "elasticvue-ui")
    .WithEnvironment("ELASTICVUE_CLUSTERS", elasticvueClusters)
    .WithLifetime(ContainerLifetime.Persistent);

var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("airbnb_redis_data")
    .WithEndpoint("tcp", e => {
        e.Port = 6379;
        e.TargetPort = 6379;
    });

// 2. Debezium (CDC)
var debezium = builder.AddContainer("debezium", "docker.io/debezium/connect:2.5")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithHttpEndpoint(8083, 8083, "http")
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

// 4. Microservices (VSA Architecture)
var propSvc = builder.AddProject<Projects.Airbnb_PropertyService>("propertyservice")
    .WithDefaultServiceConfig()
    .WithReference(propDb)
    .WithReference(rabbit)
    .WaitFor(propDb)
    .WaitFor(rabbit);

var bookSvc = builder.AddProject<Projects.Airbnb_BookingService>("bookingservice")
    .WithDefaultServiceConfig()
    .WithReference(bookDb)
    .WithReference(rabbit)
    .WithReference(kafka)
    .WithReference(propSvc)
    .WaitFor(bookDb)
    .WaitFor(rabbit)
    .WaitFor(kafka);

var userSvc = builder.AddProject<Projects.Airbnb_UserService>("userservice")
    .WithDefaultServiceConfig()
    .WithReference(userDb)
    .WithReference(rabbit)
    .WithReference(propSvc)   // needed for dashboard: property stats + recent activity
    .WithReference(bookSvc)   // needed for dashboard: booking stats + revenue chart
    .WaitFor(userDb)
    .WaitFor(rabbit);

var paySvc = builder.AddProject<Projects.Airbnb_PaymentService>("paymentservice")
    .WithDefaultServiceConfig()
    .WithReference(payDb)
    .WithReference(rabbit)
    .WithReference(propSvc)   // for country master-data (tax, gateway)
    .WithReference(userSvc)   // for host basic info lookup
    .WithReference(bookSvc)   // for booking → guest lookup on admin payments
    .WaitFor(payDb)
    .WaitFor(rabbit);

var searchSvc = builder.AddProject<Projects.Airbnb_SearchService>("searchservice")
    .WithDefaultServiceConfig()
    .WithReference(elasticsearch)
    .WithReference(kafka)
    .WithReference(redis)
    .WaitFor(elasticsearch)
    .WaitFor(kafka)
    .WaitFor(redis);

var chatSvc = builder.AddProject<Projects.Airbnb_ChatService>("chatservice")
    .WithDefaultServiceConfig()
    .WithReference(chatDb)
    .WithReference(rabbit)
    .WithReference(redis)
    .WithReference(propSvc)
    .WithReference(userSvc)
    .WaitFor(chatDb)
    .WaitFor(rabbit)
    .WaitFor(redis);

// 5. API Gateway (YARP)
var gateway = builder.AddProject<Projects.Airbnb_Gateway>("gateway")
    .WithDefaultServiceConfig()
    .WithReference(userSvc)
    .WithReference(propSvc)
    .WithReference(bookSvc)
    .WithReference(paySvc)
    .WithReference(searchSvc)
    .WithReference(chatSvc);


builder.Build().Run();
