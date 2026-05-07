using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.ConfirmPayment;

public sealed class Handler(
    PaymentDbContext db,
    IPublishEndpoint publishEndpoint,
    ILogger<Handler> logger)
    : ICommandHandler<Command, bool>
{
    public async ValueTask<bool> Handle(Command req, CancellationToken ct)
    {
        var payment = await db.Payments.FirstOrDefaultAsync(p => p.Id == req.PaymentId, ct);

        if (payment == null)
        {
            logger.LogWarning("Payment {PaymentId} not found during confirmation", req.PaymentId);
            return false;
        }

        if (payment.Status != PaymentStatus.Pending)
        {
            logger.LogInformation("Payment {PaymentId} already in status {Status}, skipping", payment.Id, payment.Status);
            return true;
        }

        payment.MarkAsSuccess(req.TransactionId);
        await db.SaveChangesAsync(ct);

        // Publish Event for BookingService
        await publishEndpoint.Publish(new PaymentSucceededEvent(
            payment.Id,
            payment.BookingId,
            payment.Amount,
            payment.Currency,
            req.TransactionId
        ), ct);

        logger.LogInformation("Payment {PaymentId} confirmed and SucceededEvent published for Booking {BookingId}", payment.Id, payment.BookingId);
        return true;
    }
}
