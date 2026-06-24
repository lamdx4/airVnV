namespace Airbnb.Infrastructure.Media;

public class MockMediaProvider : IMediaProvider
{
    public SignatureResponse GenerateUploadSignature(string folder, string? publicId = null)
    {
        Console.WriteLine($"MOCK MEDIA: Generated signature for folder {folder}");
        return new SignatureResponse(
            Signature: "mock-signature",
            Timestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ApiKey: "mock-api-key",
            CloudName: "mock-cloud-name",
            Folder: folder,
            PublicId: publicId
        );
    }

    public Task<MediaUploadResult> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default)
    {
        var publicId = $"{folder}/{Guid.NewGuid()}";
        // Return a dummy image URL from picsum, seeded by the Guid so it stays consistent but random per image.
        var mockUrl = new Uri($"https://picsum.photos/seed/{Guid.NewGuid()}/800/600");
        
        Console.WriteLine($"MOCK MEDIA: Simulated upload for {fileName}. Returning {mockUrl}");
        return Task.FromResult(new MediaUploadResult(mockUrl, publicId));
    }

    public Task<bool> DeleteAsync(string publicId, CancellationToken ct = default)
    {
        Console.WriteLine($"MOCK MEDIA: Simulated delete for {publicId}");
        return Task.FromResult(true);
    }
}
