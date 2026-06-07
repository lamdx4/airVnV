using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetBookingBasicInfos;

public record Request(List<Guid> Ids);

public record BookingBasicInfoItem(Guid BookingId, decimal TotalPrice, string CurrencyCode, string CountryCode, Guid GuestId);

public record Response(List<BookingBasicInfoItem> Items);

public class Endpoint(BookingDbContext db) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/internal/bookings/basic-infos");
        AllowAnonymous();
        Summary(s => s.Summary = "Internal: batch lookup of booking basic info by ids");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var ids = req.Ids?.Distinct().ToList() ?? new List<Guid>();
        if (ids.Count == 0)
        {
            await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(new Response(new())), cancellation: ct);
            return;
        }

        var items = await db.Bookings
            .AsNoTracking()
            .Where(b => ids.Contains(b.Id))
            .Select(b => new BookingBasicInfoItem(b.Id, b.TotalPrice, b.CurrencyCode, b.CountryCode, b.GuestId))
            .ToListAsync(ct);

        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(new Response(items)), cancellation: ct);
    }
}
