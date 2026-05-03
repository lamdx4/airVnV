using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Airbnb.Infrastructure.Media;

public class CloudinaryMediaProvider : IMediaProvider
{
    private readonly MediaOptions _options;
    private readonly Cloudinary _cloudinary;

    public CloudinaryMediaProvider(IOptions<MediaOptions> options)
    {
        _options = options.Value;
        
        var account = new Account(
            _options.CloudName,
            _options.ApiKey,
            _options.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }

    public SignatureResponse GenerateUploadSignature(string folder, string? publicId = null)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        var parameters = new Dictionary<string, object>
        {
            { "folder", folder },
            { "timestamp", timestamp }
        };

        if (!string.IsNullOrEmpty(publicId))
        {
            parameters.Add("public_id", publicId);
        }

        var signature = _cloudinary.Api.SignParameters(parameters);

        return new SignatureResponse(
            signature,
            timestamp,
            _options.ApiKey,
            _options.CloudName,
            folder
        );
    }

    public async Task<MediaUploadResult> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = folder,
            // Can add default transformation here if needed
            Transformation = new Transformation().Quality("auto").FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, ct);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
        }

        return new MediaUploadResult(
            uploadResult.SecureUrl,
            uploadResult.PublicId
        );
    }

    public async Task<bool> DeleteAsync(string publicId, CancellationToken ct = default)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        
        return result.Result == "ok";
    }
}
