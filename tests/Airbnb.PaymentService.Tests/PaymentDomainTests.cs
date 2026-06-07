using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Tests;

/// <summary>
/// Tests for Payment domain — mapped to UC-C1 AC-8 (payment detail), BR-003-R2/R3/R4/R5 (refund rules)
/// </summary>
public class PaymentDomainTests
{
    [Fact]
    public void MarkAsRefunded_Success_Payment_Transitions_To_Refunded()
    {
        // BR-003-R2: Refund allowed on Success payments
        var payment = CreateSuccessfulPayment();
        payment.MarkAsRefunded();
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
    }

    [Fact]
    public void MarkAsRefunded_PartiallyRefunded_Payment_Transitions_To_Refunded()
    {
        // BR-003-R2: Refund allowed on PartiallyRefunded payments
        var payment = CreateSuccessfulPayment();
        payment.MarkAsPartiallyRefunded();
        payment.MarkAsRefunded();
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
    }

    [Fact]
    public void MarkAsRefunded_Pending_Payment_Throws()
    {
        // BR-003-R2: Refund NOT allowed on Pending payments
        var payment = CreatePendingPayment();
        Assert.Throws<InvalidOperationException>(() => payment.MarkAsRefunded());
    }

    [Fact]
    public void MarkAsRefunded_Failed_Payment_Throws()
    {
        var payment = CreatePendingPayment();
        payment.MarkAsFailed();
        Assert.Throws<InvalidOperationException>(() => payment.MarkAsRefunded());
    }

    [Fact]
    public void MarkAsPartiallyRefunded_Success_Payment_Transitions_To_PartiallyRefunded()
    {
        // BR-003-R22: Partial refund changes Payment status to PartiallyRefunded
        var payment = CreateSuccessfulPayment();
        payment.MarkAsPartiallyRefunded();
        Assert.Equal(PaymentStatus.PartiallyRefunded, payment.Status);
    }

    [Fact]
    public void MarkAsPartiallyRefunded_NonSuccess_Payment_Throws()
    {
        // Only Success can be partially refunded
        var payment = CreatePendingPayment();
        Assert.Throws<InvalidOperationException>(() => payment.MarkAsPartiallyRefunded());
    }

    [Fact]
    public void PaymentStatus_Enum_Has_Refunded_And_PartiallyRefunded()
    {
        Assert.True(Enum.IsDefined(typeof(PaymentStatus), PaymentStatus.Refunded));
        Assert.True(Enum.IsDefined(typeof(PaymentStatus), PaymentStatus.PartiallyRefunded));
    }

    private static Payment CreatePendingPayment()
    {
        return Payment.Create(Guid.NewGuid(), 100m, "VND");
    }

    private static Payment CreateSuccessfulPayment()
    {
        var payment = Payment.Create(Guid.NewGuid(), 100m, "VND");
        payment.Initiate("https://pay.example.com", DateTimeOffset.UtcNow.AddMinutes(15));
        payment.MarkAsSuccess("TXN-12345");
        return payment;
    }
}
