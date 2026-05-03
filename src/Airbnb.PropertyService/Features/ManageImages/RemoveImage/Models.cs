using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Airbnb.PropertyService.Features.ManageImages.RemoveImage;

public record Request(Guid PropertyId, Guid ImageId) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record Response(Guid Id, string Status);
