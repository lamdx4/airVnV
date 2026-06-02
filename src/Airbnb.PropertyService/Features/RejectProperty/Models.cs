using FastEndpoints;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Airbnb.PropertyService.Features.RejectProperty;

/// <summary>Admin only: PendingReview → Rejected</summary>
public record Request(Guid PropertyId, string Reason) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record Response(Guid Id, string Status, string Reason);

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MinimumLength(10).WithMessage("Rejection reason must be at least 10 characters.")
            .MaximumLength(1000);
    }
}
