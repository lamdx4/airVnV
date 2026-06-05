using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPayments;

public record Request(
    [property: BindFrom("page")] int PageNumber = 1,
    [property: BindFrom("pageSize")] int PageSize = 10,
    string? Search = null,
    int? Status = null,
    string? FromDate = null,
    string? ToDate = null,
    string? SortBy = null,
    string? SortOrder = null
) : IQuery<PagedResponse<AdminPaymentResponse>>;

public record AdminPaymentResponse(
    Guid Id,
    Guid BookingId,
    decimal Amount,
    string Currency,
    int Status,
    string? TransactionId,
    string Provider,
    string? GuestName,
    string? HostName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt
);

public record PagedResponse<T>(
    List<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
