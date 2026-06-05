using Mediator;

namespace Airbnb.PaymentService.Features.GeneratePayouts;

public record Command : ICommand<GeneratePayoutsResponse>;

public record GeneratePayoutsResponse(int PayoutsGenerated, int BookingsProcessed);
