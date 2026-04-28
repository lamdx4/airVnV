Environment.SetEnvironmentVariable("DOTNET_ASPIRE_CONTAINER_RUNTIME", "podman");
var builder = DistributedApplication.CreateBuilder(args);

// 1. Hạ tầng Dữ liệu (Infrastructure)
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("airbnb_pg_data")
    .WithEnvironment("POSTGRES_INITDB_ARGS", "-c wal_level=logical");

var userDb = postgres.AddDatabase("userdb");
var propDb = postgres.AddDatabase("propdb");
var bookDb = postgres.AddDatabase("bookdb");
var payDb = postgres.AddDatabase("paydb");

var kafka = builder.AddKafka("kafka")
    .WithDataVolume("airbnb_kafka_data")
    .WithKafkaUI()
    .WithEnvironment("KAFKA_HEAP_OPTS", "-Xms512m -Xmx512m");

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithDataVolume("airbnb_es_data")
    .WithEnvironment("ES_JAVA_OPTS", "-Xms512m -Xmx512m");

// 2. Debezium (CDC)
var debezium = builder.AddContainer("debezium", "docker.io/debezium/connect:2.5")
    .WithEnvironment("BOOTSTRAP_SERVERS", "kafka:9092")
    .WithEnvironment("GROUP_ID", "1")
    .WithEnvironment("CONFIG_STORAGE_TOPIC", "my_connect_configs")
    .WithEnvironment("OFFSET_STORAGE_TOPIC", "my_connect_offsets")
    .WithEnvironment("STATUS_STORAGE_TOPIC", "my_connect_statuses")
    .WithEnvironment("KEY_CONVERTER", "org.apache.kafka.connect.json.JsonConverter")
    .WithEnvironment("VALUE_CONVERTER", "org.apache.kafka.connect.json.JsonConverter")
    .WithReference(postgres)
    .WithReference(kafka)
    .WaitFor(kafka);

// 3. Worker Configurator
builder.AddProject<Projects.Airbnb_Infrastructure_Configurator>("debezium-configurator");

// 4. Microservices (VSA Architecture)
var userSvc = builder.AddProject<Projects.Airbnb_UserService>("userservice")
    .WithReference(userDb);

var propSvc = builder.AddProject<Projects.Airbnb_PropertyService>("propertyservice")
    .WithReference(propDb)
    .WithReference(kafka);

var bookSvc = builder.AddProject<Projects.Airbnb_BookingService>("bookingservice")
    .WithReference(bookDb)
    .WithReference(kafka);

var paySvc = builder.AddProject<Projects.Airbnb_PaymentService>("paymentservice")
    .WithReference(payDb)
    .WithReference(kafka);

var searchSvc = builder.AddProject<Projects.Airbnb_SearchService>("searchservice")
    .WithReference(elasticsearch)
    .WithReference(kafka);

// 5. API Gateway (YARP)
var gateway = builder.AddProject<Projects.Airbnb_Gateway>("gateway")
    .WithReference(userSvc)
    .WithReference(propSvc)
    .WithReference(bookSvc)
    .WithReference(paySvc)
    .WithReference(searchSvc);

// 6. Frontend (React Vite)
builder.AddViteApp("frontend", "../airbnb-web")
    .WithReference(gateway)
    .WithEnvironment("VITE_API_URL", gateway.GetEndpoint("http"));


builder.Build().Run();
