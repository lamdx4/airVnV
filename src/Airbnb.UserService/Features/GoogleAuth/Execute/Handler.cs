using Mediator;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Google.Apis.Auth;
using Airbnb.UserService.Infrastructure;
using Airbnb.UserService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.GoogleAuth.Execute;

public sealed class Handler(UserDbContext _db, IConfiguration _config, ILogger<Handler> _logger) : ICommandHandler<Request, ApiResponse<Response>>
{
    public async ValueTask<ApiResponse<Response>> Handle(Request req, CancellationToken ct)
    {
        try 
        {
            var clientId = _config["Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId is missing.");
            
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [clientId]
            };

            var validateTask = GoogleJsonWebSignature.ValidateAsync(req.IdToken, settings);
            if (await Task.WhenAny(validateTask, Task.Delay(TimeSpan.FromSeconds(10), ct)) != validateTask)
                throw new TimeoutException("Google token validation timed out after 10 seconds.");

            var payload = await validateTask;
            
            string email = payload.Email;
            string fullName = payload.Name ?? "Google User";
            string googleId = payload.Subject;

            var user = await _db.Users
                .Include(u => u.Profile)
                .Include(u => u.Logins)
                .FirstOrDefaultAsync(u => u.Email == email, ct);
            
            if (user == null)
            {
                user = new User(email, UserRole.User, fullName, AuthProvider.Google, googleId);
                _db.Users.Add(user);
            }
            else if (!user.Logins.Any(l => l.Provider == AuthProvider.Google && l.ProviderKey == googleId))
            {
                user.AddLogin(AuthProvider.Google, googleId);
            }

            var key = _config["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT Signing Key is missing.");
            var accessToken = JwtBearer.CreateToken(o =>
            {
                o.SigningKey = key;
                o.ExpireAt = DateTime.UtcNow.AddMinutes(15);
                o.User.Claims.Add(new Claim("UserId", user.Id.ToString()));
                o.User.Claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
            });

            var refreshToken = Guid.CreateVersion7().ToString("N");

            // Optimistic Concurrency (DbUpdateConcurrencyException)
            int maxRetries = 3;
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    var token = new UserRefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(7), req.UserAgent, req.IpAddress);
                    _db.UserRefreshTokens.Add(token);
                    await _db.SaveChangesAsync(ct);
                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning($"[Concurrency Error] Entities involved: {string.Join(", ", ex.Entries.Select(e => e.Entity.GetType().Name))}");
                    
                    if (retry == maxRetries - 1)
                        throw;

                    var targetId = user.Id;
                    _db.ChangeTracker.Clear();
                    user = await _db.Users
                        .Include(u => u.Profile)
                        .Include(u => u.Logins)
                        .FirstOrDefaultAsync(u => u.Id == targetId, ct)
                        ?? throw new InvalidOperationException("User disappeared during concurrency retry.");
                }
            }

            return ApiResponse<Response>.SuccessResult(new Response(accessToken, refreshToken, user.Profile.FullName, user.Email, user.Role), "Google authentication successful");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[GoogleAuth] ERROR: {ex.Message}");
            throw;
        }
    }
}
