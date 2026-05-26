namespace Airbnb.PaymentService.Features.PlatformFee;

public class Handler
{
    // In-memory configuration - in production, store in database or config service
    private static decimal _hostFeePercent = 10.0m;
    private static decimal _guestFeePercent = 0.0m;
    private static DateTimeOffset _lastUpdatedAt = DateTimeOffset.UtcNow;

    public Response Handle(Request request, CancellationToken ct)
    {
        // Update if provided
        if (request.HostFeePercent.HasValue)
        {
            if (request.HostFeePercent.Value < 0 || request.HostFeePercent.Value > 100)
                throw new ArgumentException("HostFeePercent must be between 0 and 100");
            _hostFeePercent = request.HostFeePercent.Value;
            _lastUpdatedAt = DateTimeOffset.UtcNow;
        }

        if (request.GuestFeePercent.HasValue)
        {
            if (request.GuestFeePercent.Value < 0 || request.GuestFeePercent.Value > 100)
                throw new ArgumentException("GuestFeePercent must be between 0 and 100");
            _guestFeePercent = request.GuestFeePercent.Value;
            _lastUpdatedAt = DateTimeOffset.UtcNow;
        }

        return new Response
        {
            HostFeePercent = _hostFeePercent,
            GuestFeePercent = _guestFeePercent,
            DefaultPlatformFeePercent = _hostFeePercent + _guestFeePercent,
            LastUpdatedBy = "admin", // TODO: Get from JWT claims
            LastUpdatedAt = _lastUpdatedAt
        };
    }
}
