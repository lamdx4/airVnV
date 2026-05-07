using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.FailPayment;

public sealed class Handler(
    PaymentDbContext db,
    ILogger<Handler> logger)
    : ICommandHandler<Command, bool>
{
    public async ValueTask<bool> Handle(Command req, CancellationToken ct)
    {
        var payment = await db.Payments.FirstOrDefaultAsync(p => p.Id == req.PaymentId, ct);

        if (payment == null)
        {
            logger.LogWarning("Payment {PaymentId} not found during failure processing", req.PaymentId);
            return false;
        }

        if (payment.Status != PaymentStatus.Pending)
        {
            logger.LogInformation("Payment {PaymentId} already in status {Status}, skipping failure update", payment.Id, payment.Status);
            return true;
        }

        payment.MarkAsFailed(); 
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Payment {PaymentId} marked as Failed and event staged for Booking {BookingId}", payment.Id, payment.BookingId);
        return true;
    }
}
