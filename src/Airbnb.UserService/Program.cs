using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;
using Airbnb.SharedKernel.Infrastructure;
using Airbnb.UserService.Infrastructure.Messaging;
using Airbnb.ServiceDefaults.Infrastructure;

using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Airbnb.Infrastructure.Media;
using MassTransit;
using Mediator;

[assembly: MediatorOptions(ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

// Initialize Firebase Admin SDK for Push Notifications
if (File.Exists("firebase-service-account.json"))
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = CredentialFactory.FromFile<ServiceAccountCredential>("firebase-service-account.json")
            .ToGoogleCredential()
    });
}

builder.AddServiceDefaults();

// Database registration
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("userdb"));
});
// builder.EnrichNpgsqlDbContext<UserDbContext>();
// NOTE: Bị comment vì NpgsqlHealthCheck hang khi DCP proxy (port 5433)
// không forward PostgreSQL protocol → health check timeout → TaskCanceledException

builder.Services.AddMediaServices(builder.Configuration);

builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing from configuration."));
builder.Services.AddAuthorization();
builder.Services.AddMemoryCache();

// Thêm Mediator
builder.Services.AddMediator();

// Event Architecture
builder.Services.AddScoped<IIntegrationEventMapper, UserIntegrationEventMapper>();
builder.Services.AddScoped<IIntegrationEventBridge, UserIntegrationEventBridge>();
builder.Services.AddScoped<IDomainEventPolicyExecutor, UserDomainEventPolicyExecutor>();

// Thêm MassTransit + RabbitMQ + EF Core Outbox
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddEntityFrameworkOutbox<UserDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(5);
        o.UsePostgres();
        o.UseBusOutbox();
        // Không block startup nếu bus chưa sẵn sàng
        o.DisableInboxCleanupService();
    });

    x.UsingRabbitMq((ctx, cfg) =>
    {
        var rabbitConnectionString = builder.Configuration.GetConnectionString("rabbit");
        if (!string.IsNullOrEmpty(rabbitConnectionString))
        {
            cfg.Host(rabbitConnectionString);
        }
        else
        {
            cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", h =>
            {
                h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
                h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
            });
        }
        // UserService không có Consumer → không cần declare queues
        // cfg.ConfigureEndpoints(ctx); ← đây là nguyên nhân gây deadlock
    });
});

// Không đợi MassTransit bus hoàn toàn start mới accept HTTP request
builder.Services.Configure<HostOptions>(opts =>
{
    opts.ServicesStartConcurrently = true;
    opts.StartupTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();



builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
        UserJsonContext.Default, 
        new DefaultJsonTypeInfoResolver());
});

var app = builder.Build();

// ExceptionHandlingMiddleware phải là FIRST – wrap toàn bộ pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.UseFastEndpoints(c =>
{
    c.Serializer.Options.TypeInfoResolver = JsonTypeInfoResolver.Combine(
        UserJsonContext.Default, 
        new DefaultJsonTypeInfoResolver());
});

if (app.Environment.IsDevelopment())
    app.UseSwaggerGen();

app.MapDefaultEndpoints();

// 6. DB Migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<UserDbContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();
