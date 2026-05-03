namespace Airbnb.Infrastructure.Media;

public record SignatureResponse(
    string Signature,
    long Timestamp,
    string ApiKey,
    string CloudName,
    string Folder
);

public interface IMediaProvider
{
    /// <summary>
    /// Tạo chữ ký upload cho Cloudinary (Signed Upload)
    /// </summary>
    /// <param name="folder">Thư mục lưu trữ (avatars, properties...)</param>
    /// <param name="publicId">ID định danh (có thể dùng để map ownership)</param>
    SignatureResponse GenerateUploadSignature(string folder, string? publicId = null);
}
