using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;
using Airbnb.PropertyService.Domain.Enums;
using FluentValidation;

namespace Airbnb.PropertyService.Features.ManageImages.AddImage;

public record Request(
    Guid PropertyId,
    IFormFile File, // Get file directly
    ImageType Type,
    int DisplayOrder) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record Response(Guid Id, string Url);

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.File).NotEmpty().WithMessage("File is required.");
        
        // Validate file size (e.g., < 5MB)  
        RuleFor(x => x.File.Length).LessThanOrEqualTo(5 * 1024 * 1024)
            .WithMessage("File size must be less than 5MB.");

        // Validate file type
        RuleFor(x => x.File.ContentType).Must(x => x is "image/jpeg" or "image/png" or "image/webp")
            .WithMessage("Only JPEG, PNG and WebP images are allowed.");
            
        RuleFor(x => x.Type).IsInEnum();
    }
}
