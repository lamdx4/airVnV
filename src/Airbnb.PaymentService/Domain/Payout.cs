using System.ComponentModel.DataAnnotations;
using Airbnb.SharedKernel;

namespace Airbnb.PaymentService.Domain;

public enum PayoutStatus { Pending, Approved, Processing, Completed, Failed, Cancelled }

public class Payout : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid HostId { get; private set; }
    public decimal TotalEarnings { get; private set; }
    public decimal PlatformFee { get; private set; }
    public decimal PayoutAmount { get; private set; }
    public string Currency { get; private set; } = default!;
    public PayoutStatus Status { get; private set; }
    public int ItemCount { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private readonly List<PayoutItem> _items = [];
    public IReadOnlyList<PayoutItem> Items => _items.AsReadOnly();

    private Payout() { } // EF Core

    public static Payout Create(Guid hostId, string currency, List<PayoutItem> items)
    {
        if (hostId == Guid.Empty) throw new ArgumentException("HostId cannot be empty.");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.");
        if (items == null || items.Count == 0) throw new ArgumentException("Payout must contain at least one item.");

        var totalEarnings = items.Sum(i => i.BookingTotal);
        var platformFee = items.Sum(i => i.ServiceFee);
        var payoutAmount = totalEarnings - platformFee;

        var payout = new Payout
        {
            Id = Guid.CreateVersion7(),
            HostId = hostId,
            TotalEarnings = totalEarnings,
            PlatformFee = platformFee,
            PayoutAmount = payoutAmount,
            Currency = currency.ToUpperInvariant(),
            Status = PayoutStatus.Pending,
            ItemCount = items.Count,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        foreach (var item in items)
        {
            item.SetPayoutId(payout.Id);
            payout._items.Add(item);
        }

        return payout;
    }

    public void Approve(Guid approvedBy)
    {
        if (approvedBy == Guid.Empty) throw new ArgumentException("ApprovedBy cannot be empty.");
        if (Status != PayoutStatus.Pending)
            throw new InvalidOperationException("Only Pending payouts can be approved.");

        Status = PayoutStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTimeOffset.UtcNow;
        Version++;
    }

    public void Execute()
    {
        if (Status != PayoutStatus.Approved)
            throw new InvalidOperationException("Only Approved payouts can be executed.");

        Status = PayoutStatus.Processing;
        Version++;
    }

    public void MarkAsCompleted()
    {
        if (Status != PayoutStatus.Processing)
            throw new InvalidOperationException("Only Processing payouts can be marked as completed.");

        Status = PayoutStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
        Version++;
    }

    public void MarkAsFailed()
    {
        if (Status != PayoutStatus.Processing)
            throw new InvalidOperationException("Only Processing payouts can be marked as failed.");

        Status = PayoutStatus.Failed;
        Version++;
    }

    public void Cancel()
    {
        if (Status != PayoutStatus.Pending && Status != PayoutStatus.Failed)
            throw new InvalidOperationException("Only Pending or Failed payouts can be cancelled.");

        Status = PayoutStatus.Cancelled;
        Version++;
    }

    public void Retry()
    {
        if (Status != PayoutStatus.Failed)
            throw new InvalidOperationException("Only Failed payouts can be retried.");

        Status = PayoutStatus.Processing;
        Version++;
    }
}
