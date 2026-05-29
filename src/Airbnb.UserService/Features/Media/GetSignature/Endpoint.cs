using FastEndpoints;
using Airbnb.Infrastructure.Media;
using Airbnb.ServiceDefaults.Infrastructure;
using System.Security.Claims;

using Airbnb.UserService.Features;

namespace Airbnb.UserService.Features.Media.GetSignature;

public class Endpoint(IMediaProvider mediaProvider) : Endpoint<Request, ApiResponse<SignatureResponse>>
{
    public override void Configure()
    {
        Get("/media/signature");
        Group<AuthGroup>(); // Chỉ người dùng đã đăng nhập mới lấy được chữ ký
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = User.FindFirstValue("UserId");
        
        // Nếu là upload avatar, cố định ID để Cloudinary ghi đè file cũ (Overwrite) -> Chống rác dung lượng.
        var publicId = req.Folder.Equals("avatars", StringComparison.OrdinalIgnoreCase) 
            ? $"{req.Folder}/{userId}" 
            : $"{req.Folder}/{userId}_{Guid.CreateVersion7():N}";

        var signature = mediaProvider.GenerateUploadSignature(req.Folder, publicId);

        await Send.ResponseAsync(ApiResponse<SignatureResponse>.SuccessResult(signature), cancellation: ct);
    }
}
