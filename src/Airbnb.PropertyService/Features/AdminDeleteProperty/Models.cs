using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Airbnb.PropertyService.Features.AdminDeleteProperty;

/// <summary>Admin only: Delete any property regardless of status</summary>
public record Request(Guid PropertyId) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record Response(Guid Id, string Message);
