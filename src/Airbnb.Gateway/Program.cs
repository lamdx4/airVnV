using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy.Transforms;
using System.IdentityModel.Tokens.Jwt;

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 2. YARP with Header Transformation
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver()
    .AddTransforms(transformBuilder =>
    {
        transformBuilder.AddRequestTransform(transformContext =>
        {
            var httpContext = transformContext.HttpContext;
            string? userId = null;

            // 1. Lấy từ HttpContext.User (nếu Route có Policy)
            var user = httpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                userId = user.FindFirst("UserId")?.Value;
            }

            // 2. Giải mã thủ công để hỗ trợ các Anonymous Routes có gửi kèm token
            if (string.IsNullOrEmpty(userId) && httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var authStr = authHeader.ToString();
                if (authStr.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authStr.Substring("Bearer ".Length).Trim();
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        if (handler.CanReadToken(token))
                        {
                            var jwtToken = handler.ReadJwtToken(token);
                            userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId" || c.Type == "sub")?.Value;
                        }
                    }
                    catch
                    {
                        // Bỏ qua lỗi giải mã token
                    }
                }
            }

            if (!string.IsNullOrEmpty(userId))
            {
                // Chuyển UserId vào Header để các Microservice phía dưới sử dụng
                transformContext.ProxyRequest.Headers.Remove("X-User-Id");
                transformContext.ProxyRequest.Headers.Add("X-User-Id", userId);
            }

            return ValueTask.CompletedTask;
        });
    });

// 3. Rate Limiting & Caching (Giữ nguyên từ trước)
builder.Services.AddRateLimiter(options => { /* ... */ });
builder.Services.AddOutputCache(options => { /* ... */ });

var app = builder.Build();

app.UseCors("AllowAll");
app.UseWebSockets();

// Hỗ trợ Google Auth Popups và FedCM
app.Use((context, next) =>
{
    context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
    return next();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.UseOutputCache();

app.MapReverseProxy();

app.MapDefaultEndpoints();
app.Run();
