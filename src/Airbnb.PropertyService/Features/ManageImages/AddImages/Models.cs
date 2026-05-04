using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;
using Airbnb.PropertyService.Domain.Enums;
using FluentValidation;

namespace Airbnb.PropertyService.Features.ManageImages.AddImages;

public record Request(
    Guid PropertyId,
    List<IFormFile> Files,
    ImageType Type) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record ImageResponse(Guid Id, string Url);
public record Response(List<ImageResponse> Images);

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.Files).NotEmpty().WithMessage("At least one file is required.");
        
        RuleForEach(x => x.Files).ChildRules(file => {
            file.RuleFor(x => x.Length).LessThanOrEqualTo(5 * 1024 * 1024)
                .WithMessage("Each file size must be less than 5MB.");
            file.RuleFor(x => x.ContentType).Must(x => x is "image/jpeg" or "image/png" or "image/webp")
                .WithMessage("Only JPEG, PNG and WebP images are allowed.");
        });
            
        RuleFor(x => x.Type).IsInEnum();
    }
}
