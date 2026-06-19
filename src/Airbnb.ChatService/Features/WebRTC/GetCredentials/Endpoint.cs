using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Airbnb.ChatService.Features.WebRTC.GetCredentials;

public class Endpoint(IConfiguration config) : EndpointWithoutRequest<ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/chat/webrtc-credentials");
        AllowAnonymous(); // Mọi client đang connect vào app đều có thể lấy cấu hình để init call
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var turnUrl = config["WebRTC:TurnUrl"];
        var turnUser = config["WebRTC:TurnUsername"];
        var turnPass = config["WebRTC:TurnPassword"];

        var iceServers = new List<IceServer>
        {
            // Mặc định luôn cung cấp Google STUN server
            new IceServer("stun:stun.l.google.com:19302")
        };

        // Nếu có cấu hình TURN (từ biến môi trường hoặc appsettings), thêm vào danh sách
        if (!string.IsNullOrEmpty(turnUrl) && !string.IsNullOrEmpty(turnUser) && !string.IsNullOrEmpty(turnPass))
        {
            iceServers.Add(new IceServer(turnUrl, turnUser, turnPass));
        }

        var response = new Response(iceServers);

        await SendAsync(ApiResponse<Response>.SuccessResult(response), cancellation: ct);
    }
}
