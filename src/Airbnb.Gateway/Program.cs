using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// 1. JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyThatIsAtLeast32CharsLong!!"))
        };
    });

builder.Services.AddAuthorization();

// 2. YARP with Header Transformation
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformBuilder =>
    {
        transformBuilder.AddRequestTransform(transformContext =>
        {
            var user = transformContext.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst("UserId")?.Value;
                if (userId != null)
                {
                    // Chuyển UserId vào Header để các Microservice phía dưới sử dụng
                    transformContext.ProxyRequest.Headers.Remove("X-User-Id");
                    transformContext.ProxyRequest.Headers.Add("X-User-Id", userId);
                }
            }
            return ValueTask.CompletedTask;
        });
    });

// 3. Rate Limiting & Caching (Giữ nguyên từ trước)
builder.Services.AddRateLimiter(options => { /* ... */ });
builder.Services.AddOutputCache(options => { /* ... */ });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.UseOutputCache();

app.MapReverseProxy();

app.MapDefaultEndpoints();
app.Run();
