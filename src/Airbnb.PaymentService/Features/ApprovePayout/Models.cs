using Mediator;

namespace Airbnb.PaymentService.Features.ApprovePayout;

public record Command(Guid PayoutId) : ICommand<PayoutActionResponse>;

public record PayoutActionResponse(Guid PayoutId, int NewStatus);
