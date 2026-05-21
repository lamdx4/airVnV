var builder = DistributedApplication.CreateBuilder(args);

// Đọc config môi trường
var containerRuntime = builder.Configuration["ContainerRuntime"] ?? "docker";
Environment.SetEnvironmentVariable("DOTNET_ASPIRE_CONTAINER_RUNTIME", containerRuntime);

var kafkaHeap = builder.Configuration["KafkaHeap"] ?? "-Xms256m -Xmx256m";
var elasticHeap = builder.Configuration["ElasticHeap"] ?? "-Xms256m -Xmx256m";

// 1. Hạ tầng Dữ liệu (Infrastructure)
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("airbnb_pg_data")
    .WithEnvironment("POSTGRES_INITDB_ARGS", "-c wal_level=logical")
    .WithEndpoint("tcp", e => {
        e.Port = 5435;
        e.TargetPort = 5432;
    });

var userDb = postgres.AddDatabase("userdb");
var propDb = postgres.AddDatabase("propdb");
var bookDb = postgres.AddDatabase("bookdb");
var payDb = postgres.AddDatabase("paydb");
var chatDb = postgres.AddDatabase("chatdb");

var kafka = builder.AddKafka("kafka")
    .WithDataVolume("airbnb_kafka_data")
    .WithKafkaUI()
    .WithEnvironment("KAFKA_HEAP_OPTS", kafkaHeap);

// RabbitMQ – Domain Events + MassTransit Saga (PropertyService, BookingService, v.v.)
var rabbit = builder.AddRabbitMQ("rabbit")
    .WithDataVolume("airbnb_rabbit_data")
    .WithManagementPlugin(); // UI: http://localhost:15672

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithDataVolume("airbnb_es_data")
    .WithEnvironment("ES_JAVA_OPTS", elasticHeap);

var redis = builder.AddRedis("redis")
    .WithDataVolume("airbnb_redis_data");

// 2. Debezium (CDC)
var debezium = builder.AddContainer("debezium", "docker.io/debezium/connect:2.5")
    .WithHttpEndpoint(8083, 8083, "http")
    .WithEnvironment("BOOTSTRAP_SERVERS", "kafka:9092")
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
var userSvc = builder.AddProject<Projects.Airbnb_UserService>("userservice")
    .WithEnvironment("DOTNET_gcServer", "0") // Workstation GC giúp tiết kiệm RAM tối đa cho local
    .WithReference(userDb)
    .WithReference(rabbit)
    .WaitFor(userDb)
    .WaitFor(rabbit);

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

var paySvc = builder.AddProject<Projects.Airbnb_PaymentService>("paymentservice")
    .WithEnvironment("DOTNET_gcServer", "0")
    .WithReference(payDb)
    .WithReference(rabbit)
    .WaitFor(payDb)
    .WaitFor(rabbit);

var searchSvc = builder.AddProject<Projects.Airbnb_SearchService>("searchservice")
    .WithEnvironment("DOTNET_gcServer", "0")
    .WithReference(elasticsearch)
    .WithReference(kafka)
    .WaitFor(elasticsearch)
    .WaitFor(kafka);

var chatSvc = builder.AddProject<Projects.Airbnb_ChatService>("chatservice")
    .WithEnvironment("DOTNET_gcServer", "0")
    .WithReference(chatDb)
    .WithReference(rabbit)
    .WithReference(redis)
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

// 6. Frontend (React Vite)
builder.AddViteApp("frontend", "../airbnb-web")
    .WithEndpoint("http", e => {
        e.Port = 5173;
        e.TargetPort = 5173;
        e.IsProxied = false;
    })
    .WithReference(gateway)
    .WithEnvironment("VITE_API_URL", gateway.GetEndpoint("http"))
    .WithEnvironment("VITE_CHAT_HUB_URL", $"{gateway.GetEndpoint("http")}/hubs/chat");


builder.Build().Run();
