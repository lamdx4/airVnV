using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;
using Airbnb.PropertyService.Domain.Enums;
using FluentValidation;

namespace Airbnb.PropertyService.Features.ManageImages.AddImage;

public record Request(
    Guid PropertyId,
    string Url,
    ImageType Type,
    int DisplayOrder) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record Response(Guid Id);

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Url).NotEmpty().Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("Invalid URL format.");
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
