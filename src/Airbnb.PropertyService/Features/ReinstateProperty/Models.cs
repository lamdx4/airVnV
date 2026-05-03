using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Airbnb.PropertyService.Features.ReinstateProperty;

/// <summary>Admin only: Suspended → Published</summary>
public record Request(Guid PropertyId) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record Response(Guid Id, string Status);
