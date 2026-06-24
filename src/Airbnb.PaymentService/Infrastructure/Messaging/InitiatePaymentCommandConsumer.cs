using MassTransit;
using Airbnb.SharedKernel.Events;
using Mediator;
using Airbnb.PaymentService.Features.InitiatePayment;
using Airbnb.ServiceDefaults.Infrastructure;

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
        catch (BusinessException)
        {
            // BusinessException là lỗi xác định (booking not found, forbidden...) — không retry
            // Publish PaymentFailed để Saga cancel booking ngay
            await context.Publish(new PaymentFailedEvent(
                Guid.Empty, msg.BookingId, "PAYMENT_INITIATION_FAILED"), context.CancellationToken);
            logger.LogWarning("Business error initiating payment for Booking {BookingId}", msg.BookingId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error initiating payment for Booking {BookingId}", msg.BookingId);
            // Re-throw để MassTransit retry (transient errors như DB down, VNPay timeout)
            throw;
        }
    }
}
