using System.Text.Json;
using System.Diagnostics;
using FastEndpoints;
using FluentValidation;
using Airbnb.PaymentService.Domain;
using Airbnb.PaymentService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PaymentService.Features.ProcessPayment;

public record Request(Guid BookingId, decimal Amount);
public record Response(Guid Id, string Status, string PaymentUrl);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class Endpoint : FastEndpoints.Endpoint<Request, Response>
{
    private readonly PaymentDbContext db;

    public Endpoint(PaymentDbContext db)
    {
        this.db = db;
    }

    public override void Configure()
    {
        Post("/api/payments/process");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Staff-level: Transactional Outbox Pattern
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var payment = Payment.Create(req.BookingId, req.Amount);
            db.Payments.Add(payment);
            
            // Giả lập thanh toán thành công
            payment.MarkAsSuccess("VNP_123456");

            // Tạo Outbox Event
            var evtPayload = new { 
                BookingId = req.BookingId, 
                TransactionId = "VNP_123456",
                OccurredAt = DateTime.UtcNow
            };

            var outbox = new OutboxEvent
            {
                EventType = "PaymentSuccess",
                Payload = JsonSerializer.Serialize(evtPayload),
                TraceId = Activity.Current?.Id // Capture distributed trace ID
            };

            db.OutboxEvents.Add(outbox);

            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            string mockPaymentUrl = $"https://sandbox.vnpayment.vn/payment?amount={req.Amount}&txnId={payment.Id}";
            Response = new Response(payment.Id, payment.Status.ToString(), mockPaymentUrl);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
