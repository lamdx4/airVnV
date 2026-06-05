namespace Airbnb.PaymentService.Domain;

public class PayoutItem
{
    public Guid Id { get; private set; }
    public Guid PayoutId { get; private set; }
    public Guid BookingId { get; private set; }
    public Guid PaymentId { get; private set; }
    public decimal BookingTotal { get; private set; }
    public decimal ServiceFee { get; private set; }
    public decimal HostEarning { get; private set; }
    public DateOnly CheckIn { get; private set; }
    public DateOnly CheckOut { get; private set; }
    public string PropertyTitle { get; private set; } = default!;
    public string GuestName { get; private set; } = default!;

    private PayoutItem() { } // EF Core

    public static PayoutItem Create(
        Guid bookingId, Guid paymentId,
        decimal bookingTotal, decimal serviceFee,
        DateOnly checkIn, DateOnly checkOut,
        string propertyTitle, string guestName)
    {
        if (bookingId == Guid.Empty) throw new ArgumentException("BookingId cannot be empty.");
        if (paymentId == Guid.Empty) throw new ArgumentException("PaymentId cannot be empty.");
        if (bookingTotal <= 0) throw new ArgumentException("BookingTotal must be greater than 0.");
        if (serviceFee < 0) throw new ArgumentException("ServiceFee cannot be negative.");
        if (string.IsNullOrWhiteSpace(propertyTitle)) throw new ArgumentException("PropertyTitle is required.");
        if (string.IsNullOrWhiteSpace(guestName)) throw new ArgumentException("GuestName is required.");

        return new PayoutItem
        {
            Id = Guid.CreateVersion7(),
            BookingId = bookingId,
            PaymentId = paymentId,
            BookingTotal = bookingTotal,
            ServiceFee = serviceFee,
            HostEarning = bookingTotal - serviceFee,
            CheckIn = checkIn,
            CheckOut = checkOut,
            PropertyTitle = propertyTitle,
            GuestName = guestName,
        };
    }

    public void SetPayoutId(Guid payoutId)
    {
        PayoutId = payoutId;
    }
}
