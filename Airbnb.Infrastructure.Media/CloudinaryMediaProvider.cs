using CloudinaryDotNet;
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

        // Tạo chữ ký từ Cloudinary SDK
        var signature = _cloudinary.Api.SignParameters(parameters);

        return new SignatureResponse(
            signature,
            timestamp,
            _options.ApiKey,
            _options.CloudName,
            folder
        );
    }
}
