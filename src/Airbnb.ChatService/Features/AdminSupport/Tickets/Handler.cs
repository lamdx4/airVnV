using Airbnb.ChatService.Domain.Tickets;
using Airbnb.ChatService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.AdminSupport.Tickets;

public class Handler(AppDbContext dbContext)
{
    public async Task<TicketListResponse> GetTickets(GetTicketsRequest request, CancellationToken ct)
    {
        var query = dbContext.SupportTickets.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<TicketStatus>(request.Status, true, out var status))
            query = query.Where(t => t.Status == status);

        if (!string.IsNullOrEmpty(request.Priority) && Enum.TryParse<TicketPriority>(request.Priority, true, out var priority))
            query = query.Where(t => t.Priority == priority);

        if (!string.IsNullOrEmpty(request.Category) && Enum.TryParse<TicketCategory>(request.Category, true, out var category))
            query = query.Where(t => t.Category == category);

        if (!string.IsNullOrEmpty(request.AssignedToId) && Guid.TryParse(request.AssignedToId, out var assignedId))
            query = query.Where(t => t.AssignedToId == assignedId);

        if (!string.IsNullOrEmpty(request.ReporterId) && Guid.TryParse(request.ReporterId, out var reporterId))
            query = query.Where(t => t.ReporterId == reporterId);

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(t => t.Subject.ToLower().Contains(request.Search.ToLower()) || t.Description.ToLower().Contains(request.Search.ToLower()));

        if (request.FromDate.HasValue)
            query = query.Where(t => t.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(t => t.CreatedAt <= request.ToDate.Value);

        // Sorting
        query = request.SortBy.ToLowerInvariant() switch
        {
            "priority" => request.SortOrder == "asc" 
                ? query.OrderBy(t => t.Priority).ThenByDescending(t => t.CreatedAt)
                : query.OrderByDescending(t => t.Priority).ThenByDescending(t => t.CreatedAt),
            "status" => request.SortOrder == "asc" 
                ? query.OrderBy(t => t.Status).ThenByDescending(t => t.CreatedAt)
                : query.OrderByDescending(t => t.Status).ThenByDescending(t => t.CreatedAt),
            _ => request.SortOrder == "asc" 
                ? query.OrderBy(t => t.CreatedAt) 
                : query.OrderByDescending(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TicketSummaryResponse
            {
                Id = t.Id,
                Subject = t.Subject,
                Category = t.Category.ToString(),
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                ReporterName = t.ReporterName,
                AssignedToName = t.AssignedToName,
                CreatedAt = t.CreatedAt,
                CommentCount = t.Comments.Count
            })
            .ToListAsync(ct);

        // Get stats
        var allTickets = dbContext.SupportTickets.AsNoTracking();
        var stats = new TicketStats
        {
            TotalOpen = await allTickets.CountAsync(t => t.Status == TicketStatus.Open, ct),
            TotalInProgress = await allTickets.CountAsync(t => t.Status == TicketStatus.InProgress, ct),
            TotalResolved = await allTickets.CountAsync(t => t.Status == TicketStatus.Resolved, ct),
            TotalEscalated = await allTickets.CountAsync(t => t.Status == TicketStatus.Escalated, ct),
            HighPriorityCount = await allTickets.CountAsync(t => t.Priority == TicketPriority.High, ct),
            UrgentCount = await allTickets.CountAsync(t => t.Priority == TicketPriority.Urgent, ct)
        };

        return new TicketListResponse
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            Stats = stats
        };
    }

    public async Task<TicketResponse?> GetTicketById(Guid ticketId, CancellationToken ct)
    {
        var ticket = await dbContext.SupportTickets
            .AsNoTracking()
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == ticketId, ct);

        if (ticket == null) return null;

        return new TicketResponse
        {
            Id = ticket.Id,
            Subject = ticket.Subject,
            Description = ticket.Description,
            Category = ticket.Category.ToString(),
            Priority = ticket.Priority.ToString(),
            Status = ticket.Status.ToString(),
            BookingId = ticket.BookingId,
            PropertyId = ticket.PropertyId,
            ReporterId = ticket.ReporterId,
            ReporterEmail = ticket.ReporterEmail,
            ReporterName = ticket.ReporterName,
            IsReporterHost = ticket.IsReporterHost,
            AssignedToId = ticket.AssignedToId,
            AssignedToName = ticket.AssignedToName,
            AssignedAt = ticket.AssignedAt,
            Resolution = ticket.Resolution,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            CommentCount = ticket.Comments.Count,
            AttachmentCount = ticket.Attachments.Count,
            Comments = ticket.Comments
                .OrderBy(c => c.CreatedAt)
                .Select(c => new TicketCommentResponse
                {
                    Id = c.Id,
                    AuthorId = c.AuthorId,
                    AuthorName = c.AuthorName,
                    Content = c.Content,
                    IsInternal = c.IsInternal,
                    CreatedAt = c.CreatedAt
                })
                .ToList()
        };
    }

    public async Task<TicketResponse> AssignTicket(AssignTicketRequest request, CancellationToken ct)
    {
        var ticket = await dbContext.SupportTickets.FindAsync(new object[] { request.TicketId }, ct)
            ?? throw new InvalidOperationException("Ticket not found");

        ticket.AssignedToId = request.AssignedToId;
        ticket.AssignedAt = DateTimeOffset.UtcNow;
        ticket.Status = TicketStatus.InProgress;
        ticket.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(ct);

        return (await GetTicketById(ticket.Id, ct))!;
    }

    public async Task<TicketResponse> UpdateStatus(UpdateTicketStatusRequest request, CancellationToken ct)
    {
        var ticket = await dbContext.SupportTickets.FindAsync(new object[] { request.TicketId }, ct)
            ?? throw new InvalidOperationException("Ticket not found");

        if (!Enum.TryParse<TicketStatus>(request.Status, true, out var newStatus))
            throw new ArgumentException("Invalid status");

        ticket.Status = newStatus;
        
        if (newStatus == TicketStatus.Resolved || newStatus == TicketStatus.Closed)
        {
            ticket.Resolution = request.Resolution;
            ticket.ResolvedAt = DateTimeOffset.UtcNow;
        }

        ticket.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(ct);

        return (await GetTicketById(ticket.Id, ct))!;
    }

    public async Task<TicketCommentResponse> AddComment(AddCommentRequest request, Guid adminId, string adminName, CancellationToken ct)
    {
        var comment = new TicketComment
        {
            Id = Guid.CreateVersion7(),
            TicketId = request.TicketId,
            AuthorId = adminId,
            AuthorName = adminName,
            Content = request.Content,
            IsInternal = request.IsInternal,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.TicketComments.Add(comment);
        
        // Update ticket's UpdatedAt
        var ticket = await dbContext.SupportTickets.FindAsync(new object[] { request.TicketId }, ct);
        if (ticket != null)
        {
            ticket.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await dbContext.SaveChangesAsync(ct);

        return new TicketCommentResponse
        {
            Id = comment.Id,
            AuthorId = comment.AuthorId,
            AuthorName = comment.AuthorName,
            Content = comment.Content,
            IsInternal = comment.IsInternal,
            CreatedAt = comment.CreatedAt
        };
    }
}
