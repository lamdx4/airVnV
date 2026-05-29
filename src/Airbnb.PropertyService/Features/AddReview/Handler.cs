using Airbnb.PropertyService.Domain.Entities;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PropertyService.Features.AddReview;

public sealed class Handler(
    AppDbContext db,
    BookingServiceClient bookingServiceClient) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // 1. Kiểm tra chống spam (1 Booking chỉ có 1 Review)
        // Lưu ý: Dùng Navigation Property trên Property thì phải Include, nên ta query thẳng DbSet<Review> ẩn qua EntityType
        // Nhưng DbSet<Review> không được expose trong AppDbContext. Ta dùng db.Set<Review>()
        var existingReview = await db.Set<Review>().AnyAsync(r => r.BookingId == req.BookingId, ct);
        if (existingReview)
            throw new BusinessException("You have already reviewed this booking.", "REVIEW_ALREADY_EXISTS");

        // 2. Gọi BookingService để xác thực quyền
        var booking = await bookingServiceClient.GetBookingAsync(req.BookingId, ct);
        if (booking == null)
            throw new NotFoundException("Booking not found.");

        if (booking.GuestId != req.GuestId)
            throw new BusinessException("You are not authorized to review this booking.", "REVIEW_UNAUTHORIZED");

        if (booking.Status != "Confirmed" && booking.Status != "Completed") // Giả định Airbnb yêu cầu check-in/checkout, ở mức MVP ta check Confirmed
            throw new BusinessException("Booking must be Confirmed or Completed to review.", "REVIEW_INVALID_STATUS");

        // 3. Load Property (TUYỆT ĐỐI KHÔNG Include Reviews)
        var property = await db.Properties.FirstOrDefaultAsync(p => p.Id == req.PropertyId, ct);
        if (property == null)
            throw new NotFoundException("Property not found.");

        // 4. Gọi aggregate behavior
        try
        {
            property.AddReview(req.BookingId, req.GuestId, req.Rating, req.Comment);
        }
        catch (ArgumentException ex)
        {
            throw new BusinessException(ex.Message, "REVIEW_INVALID_INPUT");
        }

        // EF Core sẽ tự động track Review mới được add vào collection của Property
        await db.SaveChangesAsync(ct);

        // Trả về ID của review vừa tạo (lấy từ collection)
        // Vì List append-only nên phần tử cuối cùng là cái ta vừa tạo
        var newReview = property.Reviews.Last();
        
        return new Response(newReview.Id);
    }
}
