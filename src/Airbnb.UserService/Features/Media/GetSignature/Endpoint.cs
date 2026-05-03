using FastEndpoints;
using Airbnb.Infrastructure.Media;
using Airbnb.ServiceDefaults.Infrastructure;
using System.Security.Claims;

using Airbnb.UserService.Features;

namespace Airbnb.UserService.Features.Media.GetSignature;

public class Endpoint(IMediaProvider mediaProvider) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/media/signature");
        Group<AuthGroup>(); // Chỉ người dùng đã đăng nhập mới lấy được chữ ký
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = User.FindFirstValue("UserId");
        
        // Tạo PublicId duy nhất gắn với UserId để quản lý Ownership
        var publicId = $"{req.Folder}/{userId}_{Guid.NewGuid():N}";

        var signature = mediaProvider.GenerateUploadSignature(req.Folder, publicId);

        await SendAsync(ApiResponse<Response>.SuccessResult(new Response(signature)), cancellation: ct);
    }
}
