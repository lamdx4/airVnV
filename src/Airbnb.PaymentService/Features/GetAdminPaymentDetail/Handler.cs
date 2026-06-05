using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPaymentDetail;

public sealed class Handler(PaymentDbContext db) : IQueryHandler<Request, AdminPaymentDetailResponse>
{
    public async ValueTask<AdminPaymentDetailResponse> Handle(Request req, CancellationToken ct)
    {
        var payment = await db.Payments
            .AsNoTracking()
            .Where(p => p.Id == req.PaymentId)
            .Select(p => new
            {
                p.Id,
                p.BookingId,
                p.Amount,
                p.Currency,
                Status = (int)p.Status,
                p.TransactionId,
                p.PaymentUrl,
                p.ExpiresAt,
                p.CreatedAt,
            })
            .FirstOrDefaultAsync(ct);

        if (payment is null)
            throw new BusinessException("Payment not found", "PAYMENT_NOT_FOUND");

        var refunds = await db.RefundRecords
            .AsNoTracking()
            .Where(r => r.PaymentId == req.PaymentId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RefundRecordDto(
                r.Id,
                r.Amount,
                r.Reason,
                r.IsFullRefund,
                r.PerformedBy,
                r.CreatedAt
            ))
            .ToListAsync(ct);

        return new AdminPaymentDetailResponse(
            payment.Id,
            payment.BookingId,
            payment.Amount,
            payment.Currency,
            payment.Status,
            payment.TransactionId,
            "VNPay",
            payment.PaymentUrl,
            payment.ExpiresAt,
            payment.CreatedAt,
            refunds
        );
    }
}
