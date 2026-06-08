using MassTransit;
using Airbnb.SharedKernel.Events;
using Mediator;
using Airbnb.PaymentService.Features.InitiatePayment;

namespace Airbnb.PaymentService.Infrastructure.Messaging;

public class InitiatePaymentCommandConsumer(IMediator mediator, ILogger<InitiatePaymentCommandConsumer> logger) 
    : IConsumer<InitiatePaymentCommand>
{
    public async Task Consume(ConsumeContext<InitiatePaymentCommand> context)
    {
        var msg = context.Message;
        logger.LogInformation("Received InitiatePaymentCommand for Booking {BookingId}", msg.BookingId);

        try
        {
            // Internal call to the existing InitiatePayment logic
            await mediator.Send(new Request(msg.BookingId) { UserId = msg.UserId }, context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initiate payment for Booking {BookingId}", msg.BookingId);
            // In a real system, we might publish a PaymentFailedEvent here if initialization fails
        }
    }
}
