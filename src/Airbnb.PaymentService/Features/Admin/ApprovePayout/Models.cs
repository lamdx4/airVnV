using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Features.Admin.ApprovePayout;

public record Request : Mediator.ICommand<Response>
{
    public Guid Id { get; init; }
    public Guid PerformedBy { get; init; }
}

public record Response(Guid Id, PayoutStatus Status, DateTimeOffset? ApprovedAt);
