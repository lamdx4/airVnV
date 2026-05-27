using Airbnb.ChatService.Domain.Tickets;
using Airbnb.ChatService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.AdminSupport.Refunds;

public class RefundHandler(AppDbContext dbContext)
{
    public async Task<RefundListResponse> GetRefunds(GetRefundsRequest request, CancellationToken ct)
    {
        var query = dbContext.RefundRequests.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<RefundStatus>(request.Status, true, out var status))
            query = query.Where(r => r.Status == status);

        if (!string.IsNullOrEmpty(request.Type) && Enum.TryParse<RefundType>(request.Type, true, out var type))
            query = query.Where(r => r.Type == type);

        if (!string.IsNullOrEmpty(request.GuestId) && Guid.TryParse(request.GuestId, out var guestId))
            query = query.Where(r => r.GuestId == guestId);

        if (!string.IsNullOrEmpty(request.HostId) && Guid.TryParse(request.HostId, out var hostId))
            query = query.Where(r => r.HostId == hostId);

        if (!string.IsNullOrEmpty(request.BookingId) && Guid.TryParse(request.BookingId, out var bookingId))
            query = query.Where(r => r.BookingId == bookingId);

        if (request.FromDate.HasValue)
            query = query.Where(r => r.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(r => r.CreatedAt <= request.ToDate.Value);

        // Sorting
        query = request.SortBy.ToLowerInvariant() switch
        {
            "amount" => request.SortOrder == "asc" 
                ? query.OrderBy(r => r.RequestedAmount) 
                : query.OrderByDescending(r => r.RequestedAmount),
            _ => request.SortOrder == "asc" 
                ? query.OrderBy(r => r.CreatedAt) 
                : query.OrderByDescending(r => r.CreatedAt)
        };

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RefundResponse
            {
                Id = r.Id,
                BookingId = r.BookingId,
                GuestId = r.GuestId,
                HostId = r.HostId,
                Type = r.Type.ToString(),
                TotalAmount = r.TotalAmount,
                RequestedAmount = r.RequestedAmount,
                Currency = r.Currency,
                Reason = r.Reason,
                Category = r.Category.ToString(),
                Status = r.Status.ToString(),
                TransactionId = r.TransactionId,
                ProcessedById = r.ProcessedById,
                ProcessedByName = r.ProcessedByName,
                ProcessedAt = r.ProcessedAt,
                RelatedTicketId = r.RelatedTicketId,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync(ct);

        // Get stats
        var allRefunds = dbContext.RefundRequests.AsNoTracking();
        var stats = new RefundStats
        {
            TotalPending = await allRefunds.CountAsync(r => r.Status == RefundStatus.Pending, ct),
            TotalProcessing = await allRefunds.CountAsync(r => r.Status == RefundStatus.Processing, ct),
            TotalCompleted = await allRefunds.CountAsync(r => r.Status == RefundStatus.Completed, ct),
            TotalFailed = await allRefunds.CountAsync(r => r.Status == RefundStatus.Failed, ct),
            TotalPendingAmount = await allRefunds
                .Where(r => r.Status == RefundStatus.Pending || r.Status == RefundStatus.Processing)
                .SumAsync(r => r.RequestedAmount, ct)
        };

        return new RefundListResponse
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            Stats = stats
        };
    }

    public async Task<RefundResponse> ProcessRefund(ProcessRefundRequest request, Guid adminId, string adminName, CancellationToken ct)
    {
        var refund = await dbContext.RefundRequests.FindAsync(new object[] { request.RefundId }, ct)
            ?? throw new InvalidOperationException("Refund request not found");

        switch (request.Action.ToLowerInvariant())
        {
            case "approve":
                refund.Status = RefundStatus.Processing;
                break;
            case "reject":
                refund.Status = RefundStatus.Cancelled;
                refund.ProcessedById = adminId;
                refund.ProcessedByName = adminName;
                refund.ProcessedAt = DateTimeOffset.UtcNow;
                break;
            case "process":
                // In real world, this would call payment gateway to initiate refund
                refund.Status = RefundStatus.Processing;
                refund.ProcessedById = adminId;
                refund.ProcessedByName = adminName;
                refund.ProcessedAt = DateTimeOffset.UtcNow;
                break;
            default:
                throw new ArgumentException("Invalid action. Use 'approve', 'reject', or 'process'");
        }

        refund.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(ct);

        return MapToResponse(refund);
    }

    public async Task<RefundResponse> CreateRefund(CreateRefundRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<RefundType>(request.Type, true, out var refundType))
            throw new ArgumentException("Invalid refund type");

        if (!Enum.TryParse<TicketCategory>(request.Category, true, out var category))
            throw new ArgumentException("Invalid category");

        var refund = new RefundRequest
        {
            Id = Guid.CreateVersion7(),
            BookingId = request.BookingId,
            GuestId = request.GuestId,
            HostId = request.HostId,
            Type = refundType,
            TotalAmount = request.RequestedAmount, // Placeholder - should get from Booking
            RequestedAmount = request.RequestedAmount,
            Currency = "VND",
            Reason = request.Reason,
            Category = category,
            Status = RefundStatus.Pending,
            RelatedTicketId = request.RelatedTicketId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        dbContext.RefundRequests.Add(refund);
        await dbContext.SaveChangesAsync(ct);

        return MapToResponse(refund);
    }

    private static RefundResponse MapToResponse(RefundRequest r) => new()
    {
        Id = r.Id,
        BookingId = r.BookingId,
        GuestId = r.GuestId,
        HostId = r.HostId,
        Type = r.Type.ToString(),
        TotalAmount = r.TotalAmount,
        RequestedAmount = r.RequestedAmount,
        Currency = r.Currency,
        Reason = r.Reason,
        Category = r.Category.ToString(),
        Status = r.Status.ToString(),
        TransactionId = r.TransactionId,
        ProcessedById = r.ProcessedById,
        ProcessedByName = r.ProcessedByName,
        ProcessedAt = r.ProcessedAt,
        RelatedTicketId = r.RelatedTicketId,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}
