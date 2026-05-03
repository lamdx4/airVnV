namespace Airbnb.Infrastructure.Media;

public record SignatureResponse(
    string Signature,
    long Timestamp,
    string ApiKey,
    string CloudName,
    string Folder
);

public record MediaUploadResult(
    Uri Url,
    string PublicId
);

public interface IMediaProvider
{
    /// <summary>
    /// Generate upload signature for Cloudinary (Signed Upload)
    /// </summary>
    SignatureResponse GenerateUploadSignature(string folder, string? publicId = null);

    /// <summary>
    /// Upload file from client to Cloudinary via server
    /// </summary>
    Task<MediaUploadResult> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default);

    /// <summary>
    /// Delete resource on Cloudinary
    /// </summary>
    Task<bool> DeleteAsync(string publicId, CancellationToken ct = default);
}
