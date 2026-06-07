#pragma warning disable ASPIREMCP001, ASPIREPOSTGRES001 // Experimental MCP server & Postgres MCP APIs

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
    .WithEnvironment("KAFKA_HEAP_OPTS", kafkaHeap);

// RabbitMQ – Domain Events + MassTransit Saga (PropertyService, BookingService, v.v.)
var rabbit = builder.AddRabbitMQ("rabbit")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("airbnb_rabbit_data")
    .WithManagementPlugin(); // UI: http://localhost:15672

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("airbnb_es_data")
    .WithEnvironment("ES_JAVA_OPTS", elasticHeap);

var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("airbnb_redis_data");

// 2. Debezium (CDC)
// Note: On Docker Desktop for Windows, containers run in a VM. The confluent-local Kafka image
// advertises localhost:29092 which is only reachable from the host. For container-to-container
// communication, we use host.docker.internal which resolves to the host from inside containers.
var debezium = builder.AddContainer("debezium", "docker.io/debezium/connect:2.5")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithHttpEndpoint(8083, 8083, "http")
    .WithEnvironment("BOOTSTRAP_SERVERS", "host.docker.internal:29092")  // Docker Desktop host bridge
    .WithEnvironment("GROUP_ID", "1")
    .WithEnvironment("CONFIG_STORAGE_TOPIC", "my_connect_configs")
    .WithEnvironment("OFFSET_STORAGE_TOPIC", "my_connect_offsets")
    .WithEnvironment("STATUS_STORAGE_TOPIC", "my_connect_statuses")
    .WithEnvironment("KEY_CONVERTER", "org.apache.kafka.connect.json.JsonConverter")
    .WithEnvironment("VALUE_CONVERTER", "org.apache.kafka.connect.json.JsonConverter")
    .WithEnvironment("JAVA_OPTS", "-Xms256m -Xmx512m") // Giới hạn RAM cho Debezium JVM
    .WithReference(postgres)
    .WithReference(kafka)
    .WaitFor(kafka);

// 3. Worker Configurator
builder.AddProject<Projects.Airbnb_Infrastructure_Configurator>("debezium-configurator")
    .WithReference(debezium.GetEndpoint("http"))
    .WaitFor(debezium);

// 4. Microservices (VSA Architecture)
var propSvc = builder.AddProject<Projects.Airbnb_PropertyService>("propertyservice")
    .WithEnvironment("DOTNET_gcServer", "0")
    .WithReference(propDb)
    .WithReference(rabbit)
    .WaitFor(propDb)
    .WaitFor(rabbit);

var bookSvc = builder.AddProject<Projects.Airbnb_BookingService>("bookingservice")
    .WithEnvironment("DOTNET_gcServer", "0")
    .WithReference(bookDb)
    .WithReference(rabbit)
    .WithReference(kafka)
    .WaitFor(bookDb)
    .WaitFor(rabbit)
    .WaitFor(kafka);

var userSvc = builder.AddProject<Projects.Airbnb_UserService>("userservice")
    .WithEnvironment("DOTNET_gcServer", "0") // Workstation GC giúp tiết kiệm RAM tối đa cho local
    .WithReference(userDb)
    .WithReference(rabbit)
    .WithReference(propSvc)   // needed for dashboard: property stats + recent activity
    .WithReference(bookSvc)   // needed for dashboard: booking stats + revenue chart
    .WaitFor(userDb)
    .WaitFor(rabbit);

var paySvc = builder.AddProject<Projects.Airbnb_PaymentService>("paymentservice")
    .WithEnvironment("DOTNET_gcServer", "0")
    .WithReference(payDb)
    .WithReference(rabbit)
    .WithReference(userSvc)   // for host basic info lookup
    .WithReference(bookSvc)   // for booking → guest lookup on admin payments
    .WaitFor(payDb)
    .WaitFor(rabbit);

var searchSvc = builder.AddProject<Projects.Airbnb_SearchService>("searchservice")
    .WithEnvironment("DOTNET_gcServer", "0")
    .WithReference(elasticsearch)
    .WithReference(kafka)
    .WithReference(redis)
    .WaitFor(elasticsearch)
    .WaitFor(kafka)
    .WaitFor(redis);

var chatSvc = builder.AddProject<Projects.Airbnb_ChatService>("chatservice")
    .WithEnvironment("DOTNET_gcServer", "0")
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
    .WithEnvironment("DOTNET_gcServer", "0")
    .WithReference(userSvc)
    .WithReference(propSvc)
    .WithReference(bookSvc)
    .WithReference(paySvc)
    .WithReference(searchSvc)
    .WithReference(chatSvc);


builder.Build().Run();
