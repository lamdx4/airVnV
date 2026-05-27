namespace Airbnb.ChatService.Features.AdminSupport.Tickets;

public class GetTicketsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Category { get; set; }
    public string? AssignedToId { get; set; }
    public string? ReporterId { get; set; }
    public string? Search { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
}

public class GetTicketByIdRequest
{
    public Guid TicketId { get; set; }
}

public class AssignTicketRequest
{
    public Guid TicketId { get; set; }
    public Guid AssignedToId { get; set; }
}

public class UpdateTicketStatusRequest
{
    public Guid TicketId { get; set; }
    public string Status { get; set; } = default!;
    public string? Resolution { get; set; }
}

public class AddCommentRequest
{
    public Guid TicketId { get; set; }
    public string Content { get; set; } = default!;
    public bool IsInternal { get; set; } = false;
}
