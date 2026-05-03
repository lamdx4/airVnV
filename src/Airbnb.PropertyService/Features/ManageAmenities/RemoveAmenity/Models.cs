using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Airbnb.PropertyService.Features.ManageAmenities.RemoveAmenity;

public record Request(Guid PropertyId, Guid AmenityId) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record Response(Guid PropertyId, Guid AmenityId, string Status);
