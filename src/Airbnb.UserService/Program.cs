using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Airbnb.UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization.Metadata;

using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Airbnb.Infrastructure.Media;

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
builder.AddNpgsqlDbContext<UserDbContext>("userdb");

builder.Services.AddMediaServices(builder.Configuration);

builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing from configuration."));
builder.Services.AddAuthorization();
builder.Services.AddMemoryCache();

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

app.UseCors("AllowAll");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
}

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
app.Run();
