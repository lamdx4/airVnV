using FastEndpoints;
using FluentValidation;
using Airbnb.BookingService.Domain;
using Airbnb.BookingService.Infrastructure;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;

namespace Airbnb.BookingService.Features.CreateBooking;

public record Request(Guid PropertyId, DateTime CheckIn, DateTime CheckOut, decimal TotalPrice)
{
    [FromHeader("X-User-Id")]
    public Guid UserId { get; init; }
}

public record Response(Guid Id, string Status);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Thiếu thông tin người dùng từ Gateway");
        RuleFor(x => x.CheckIn).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn);
        RuleFor(x => x.TotalPrice).GreaterThan(0);
    }
}

public class Endpoint : FastEndpoints.Endpoint<Request, Response>
{
    private readonly BookingDbContext db;
    public Endpoint(BookingDbContext db) => this.db = db;

    public override void Configure()
    {
        Post("/api/bookings");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Kiểm tra xem phòng có bị trùng lịch không
        var isOverlapping = await db.Bookings.AnyAsync(b => 
            b.PropertyId == req.PropertyId &&
            b.Status != BookingStatus.Cancelled &&
            req.CheckIn < b.CheckOut && req.CheckOut > b.CheckIn, ct);

        if (isOverlapping)
        {
            await base.SendErrorsAsync(409, ct); // Conflict
            return;
        }

        var booking = Booking.Create(req.PropertyId, req.UserId,
            new DateTimeOffset(req.CheckIn, TimeSpan.Zero),
            new DateTimeOffset(req.CheckOut, TimeSpan.Zero),
            req.TotalPrice);
        
        db.Bookings.Add(booking);
        await db.SaveChangesAsync(ct);
        
        Response = new Response(booking.Id, booking.Status.ToString());
    }
}
