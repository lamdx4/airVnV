using FastEndpoints;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Airbnb.PropertyService.Features.SuspendProperty;

/// <summary>Admin only: Published → Suspended</summary>
public record Request(Guid PropertyId, string Reason) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record Response(Guid Id, string Status, string Reason);

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator() => RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
}
