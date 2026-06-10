using FastEndpoints;
using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayments;

public record Request : Mediator.IQuery<PagedResponse<AdminPaymentItem>>
{
    [BindFrom("page")] public int Page { get; init; } = 1;
    [BindFrom("pageSize")] public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public string? Search { get; init; }
    public string? From { get; init; }
    public string? To { get; init; }
    public string? SortOrder { get; init; } = "desc";
}

public record AdminPaymentItem(
    Guid Id,
    Guid BookingId,
    Guid? GuestId,
    string? GuestName,
    string? GuestEmail,
    string? GuestAvatarUrl,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    string? TransactionId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt
);

public record PagedResponse<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}
