using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayments;

/// <summary>
/// Builder Pattern: gom các bước xây dựng IQueryable&lt;Payment&gt; (filter + sort)
/// thành fluent API. Mỗi bước là optional — bỏ qua nếu input rỗng/không hợp lệ —
/// và có thể compose theo bất kỳ thứ tự nào của Where; OrderBy luôn ở cuối.
/// </summary>
public sealed class PaymentQueryBuilder
{
    private IQueryable<Payment> _query;

    public PaymentQueryBuilder(IQueryable<Payment> source)
    {
        _query = source;
    }

    public PaymentQueryBuilder WithStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status)) return this;
        if (!Enum.TryParse<PaymentStatus>(status, ignoreCase: true, out var parsed)) return this;

        _query = _query.Where(p => p.Status == parsed);
        return this;
    }

    /// <summary>
    /// Search step:
    /// - Nếu input parse được thành Guid → khớp BookingId hoặc PaymentId.
    /// - Ngược lại → substring match trên TransactionId.
    /// Là ví dụ điển hình của "complex step" trong Builder: gói nhánh logic phức tạp
    /// vào một method duy nhất, giấu hẳn khỏi handler.
    /// </summary>
    public PaymentQueryBuilder WithSearch(string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return this;

        var needle = search.Trim();
        if (Guid.TryParse(needle, out var guid))
        {
            _query = _query.Where(p => p.BookingId == guid || p.Id == guid);
        }
        else
        {
            _query = _query.Where(p => p.TransactionId != null && p.TransactionId.Contains(needle));
        }
        return this;
    }

    public PaymentQueryBuilder WithDateRange(string? from, string? to)
    {
        if (DateOnly.TryParse(from, out var fromDate))
        {
            var fromDt = new DateTimeOffset(fromDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            _query = _query.Where(p => p.CreatedAt >= fromDt);
        }
        if (DateOnly.TryParse(to, out var toDate))
        {
            var toDt = new DateTimeOffset(toDate.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);
            _query = _query.Where(p => p.CreatedAt <= toDt);
        }
        return this;
    }

    public PaymentQueryBuilder OrderByCreatedAt(string? sortOrder)
    {
        var ascending = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase);
        _query = ascending
            ? _query.OrderBy(p => p.CreatedAt)
            : _query.OrderByDescending(p => p.CreatedAt);
        return this;
    }

    public IQueryable<Payment> Build() => _query;
}
