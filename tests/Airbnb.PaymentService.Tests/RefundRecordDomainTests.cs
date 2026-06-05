using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Tests;

/// <summary>
/// Tests for RefundRecord domain — mapped to BR-003-R18/R19/R20/R21/R22 (refund business rules)
/// </summary>
public class RefundRecordDomainTests
{
    [Fact]
    public void Create_Valid_Refund_Succeeds()
    {
        var refund = RefundRecord.Create(
            Guid.NewGuid(), 50m, "Guest complaint - property not as described",
            isFullRefund: false, performedBy: Guid.NewGuid());

        Assert.Equal(50m, refund.Amount);
        Assert.False(refund.IsFullRefund);
        Assert.Equal("Guest complaint - property not as described", refund.Reason);
    }

    [Fact]
    public void Create_Full_Refund_Succeeds()
    {
        // BR-003-R21: Full refund is tracked
        var refund = RefundRecord.Create(
            Guid.NewGuid(), 100m, "Host violation - force refund",
            isFullRefund: true, performedBy: Guid.NewGuid());

        Assert.True(refund.IsFullRefund);
    }

    [Fact]
    public void Create_Refund_Zero_Amount_Throws()
    {
        // BR-003-R18: Refund amount must be positive
        Assert.Throws<ArgumentException>(() =>
            RefundRecord.Create(Guid.NewGuid(), 0m, "reason", false, Guid.NewGuid()));
    }

    [Fact]
    public void Create_Refund_Negative_Amount_Throws()
    {
        // BR-003-R18: Refund amount must be positive
        Assert.Throws<ArgumentException>(() =>
            RefundRecord.Create(Guid.NewGuid(), -10m, "reason", false, Guid.NewGuid()));
    }

    [Fact]
    public void Create_Refund_Empty_Reason_Throws()
    {
        // BR-003-R19: Reason is mandatory
        Assert.Throws<ArgumentException>(() =>
            RefundRecord.Create(Guid.NewGuid(), 50m, "", false, Guid.NewGuid()));
    }

    [Fact]
    public void Create_Refund_With_TicketId_Succeeds()
    {
        var ticketId = Guid.NewGuid();
        var refund = RefundRecord.Create(
            Guid.NewGuid(), 50m, "Dispute resolved",
            false, Guid.NewGuid(), ticketId);

        Assert.Equal(ticketId, refund.TicketId);
    }
}
