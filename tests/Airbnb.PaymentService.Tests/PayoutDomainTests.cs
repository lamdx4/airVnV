using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Tests;

/// <summary>
/// Tests for Payout domain — mapped to UC-C2 (Payout Management) acceptance criteria
/// </summary>
public class PayoutDomainTests
{
    [Fact]
    public void Create_Payout_With_Valid_Items_Succeeds()
    {
        // UC-C2 AC-1: Payout table loads with correct data
        var items = new List<PayoutItem>
        {
            PayoutItem.Create(
                Guid.NewGuid(), Guid.NewGuid(),
                1000m, 100m,
                new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 5),
                "Beach Villa", "John Guest"
            ),
        };

        var payout = Payout.Create(Guid.NewGuid(), "VND", items);

        Assert.Equal(PayoutStatus.Pending, payout.Status);
        Assert.Equal(1000m, payout.TotalEarnings);
        Assert.Equal(100m, payout.PlatformFee);
        Assert.Equal(900m, payout.PayoutAmount); // BR-003-R7
        Assert.Equal(1, payout.ItemCount);
    }

    [Fact]
    public void Approve_Pending_Payout_Transitions_To_Approved()
    {
        // UC-C2 AC-5: Approve a pending payout
        var payout = CreatePendingPayout();
        var adminId = Guid.NewGuid();

        payout.Approve(adminId);

        Assert.Equal(PayoutStatus.Approved, payout.Status);
        Assert.Equal(adminId, payout.ApprovedBy);
        Assert.NotNull(payout.ApprovedAt);
    }

    [Fact]
    public void Approve_NonPending_Payout_Throws()
    {
        // BR-003-R9: Only Pending payouts can be approved
        var payout = CreatePendingPayout();
        payout.Approve(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => payout.Approve(Guid.NewGuid()));
    }

    [Fact]
    public void Execute_Approved_Payout_Transitions_To_Processing()
    {
        // UC-C2 AC-6: Execute an approved payout
        var payout = CreatePendingPayout();
        payout.Approve(Guid.NewGuid());
        payout.Execute();

        Assert.Equal(PayoutStatus.Processing, payout.Status);
    }

    [Fact]
    public void Execute_NonApproved_Payout_Throws()
    {
        // BR-003-R10: Only Approved payouts can be executed
        var payout = CreatePendingPayout();
        Assert.Throws<InvalidOperationException>(() => payout.Execute());
    }

    [Fact]
    public void MarkAsCompleted_Processing_Payout_Sets_CompletedAt()
    {
        // UC-C2 AC-6: Payout reaches Completed
        var payout = CreatePendingPayout();
        payout.Approve(Guid.NewGuid());
        payout.Execute();
        payout.MarkAsCompleted();

        Assert.Equal(PayoutStatus.Completed, payout.Status);
        Assert.NotNull(payout.CompletedAt);
    }

    [Fact]
    public void Cancel_Pending_Payout_Transitions_To_Cancelled()
    {
        // UC-C2 AC-8: Cancel a pending payout
        var payout = CreatePendingPayout();
        payout.Cancel();

        Assert.Equal(PayoutStatus.Cancelled, payout.Status);
    }

    [Fact]
    public void Cancel_Failed_Payout_Transitions_To_Cancelled()
    {
        // BR-003-R11: Failed payouts can be cancelled
        var payout = CreatePendingPayout();
        payout.Approve(Guid.NewGuid());
        payout.Execute();
        payout.MarkAsFailed();
        payout.Cancel();

        Assert.Equal(PayoutStatus.Cancelled, payout.Status);
    }

    [Fact]
    public void Cancel_Approved_Payout_Throws()
    {
        // BR-003-R9: Only Pending or Failed can be cancelled
        var payout = CreatePendingPayout();
        payout.Approve(Guid.NewGuid());
        Assert.Throws<InvalidOperationException>(() => payout.Cancel());
    }

    [Fact]
    public void Retry_Failed_Payout_Transitions_To_Processing()
    {
        // UC-C2 AC-7: Handle failed payout with retry
        var payout = CreatePendingPayout();
        payout.Approve(Guid.NewGuid());
        payout.Execute();
        payout.MarkAsFailed();
        payout.Retry();

        Assert.Equal(PayoutStatus.Processing, payout.Status);
    }

    [Fact]
    public void Retry_NonFailed_Payout_Throws()
    {
        var payout = CreatePendingPayout();
        Assert.Throws<InvalidOperationException>(() => payout.Retry());
    }

    private static Payout CreatePendingPayout()
    {
        var items = new List<PayoutItem>
        {
            PayoutItem.Create(
                Guid.NewGuid(), Guid.NewGuid(),
                1000m, 100m,
                new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 5),
                "Beach Villa", "John Guest"
            ),
        };
        return Payout.Create(Guid.NewGuid(), "VND", items);
    }
}
