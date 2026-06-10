using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Features.Admin.MarkPayoutCompleted;

public record Request : Mediator.ICommand<Response>
{
    public Guid Id { get; init; }
}

public record Response(Guid Id, PayoutStatus Status, DateTimeOffset? CompletedAt);
