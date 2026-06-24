using Airbnb.BookingService.Domain;
using Airbnb.BookingService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.BookingService.Features.CancelBooking;

public sealed class Handler(BookingDbContext db) : ICommandHandler<Request>
{
    public async ValueTask<Unit> Handle(Request req, CancellationToken ct)
    {
        var booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == req.BookingId, ct);
        if (booking == null)
            throw new NotFoundException("Booking not found.");

        try
        {
            if (booking.Status == BookingStatus.Confirmed)
            {
                // Confirmed bookings: refund must be processed first.
                // Transitions to Refunding → Saga sends RefundPaymentCommand to PaymentService.
                // Booking will be fully Cancelled only after PaymentService confirms the refund.
                booking.CancelAndRequestRefund(req.UserId);
            }
            else
            {
                // Pending / AwaitingApproval: no payment was taken yet, cancel immediately.
                booking.Cancel(req.UserId);
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new BusinessException(ex.Message, "BOOKING_CANCEL_ERROR");
        }

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
