using Airbnb.ChatService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;

namespace Airbnb.ChatService.Features.AdminSupport.Tickets;

public class Endpoint(AppDbContext dbContext) : FastEndpoints.Endpoint<GetTicketsRequest, ApiResponse<TicketListResponse>>
{
    public override void Configure()
    {
        Get("/api/admin/support/tickets");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(GetTicketsRequest req, CancellationToken ct)
    {
        var handler = new Handler(dbContext);
        var result = await handler.GetTickets(req, ct);
        await Send.ResponseAsync(ApiResponse<TicketListResponse>.SuccessResult(result), cancellation: ct);
    }
}

public class GetByIdEndpoint(AppDbContext dbContext) : FastEndpoints.Endpoint<Guid, ApiResponse<TicketResponse>>
{
    public override void Configure()
    {
        Get("/api/admin/support/tickets/{TicketId}");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(Guid ticketId, CancellationToken ct)
    {
        var handler = new Handler(dbContext);
        var result = await handler.GetTicketById(ticketId, ct);
        if (result == null)
        {
            await Send.NotFoundAsync();
            return;
        }
        await Send.ResponseAsync(ApiResponse<TicketResponse>.SuccessResult(result), cancellation: ct);
    }
}

public class AssignEndpoint(AppDbContext dbContext) : FastEndpoints.Endpoint<AssignTicketRequest, ApiResponse<TicketResponse>>
{
    public override void Configure()
    {
        Post("/api/admin/support/tickets/assign");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(AssignTicketRequest req, CancellationToken ct)
    {
        var handler = new Handler(dbContext);
        var result = await handler.AssignTicket(req, ct);
        await Send.ResponseAsync(ApiResponse<TicketResponse>.SuccessResult(result), cancellation: ct);
    }
}

public class UpdateStatusEndpoint(AppDbContext dbContext) : FastEndpoints.Endpoint<UpdateTicketStatusRequest, ApiResponse<TicketResponse>>
{
    public override void Configure()
    {
        Put("/api/admin/support/tickets/status");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(UpdateTicketStatusRequest req, CancellationToken ct)
    {
        var handler = new Handler(dbContext);
        var result = await handler.UpdateStatus(req, ct);
        await Send.ResponseAsync(ApiResponse<TicketResponse>.SuccessResult(result), cancellation: ct);
    }
}

public class AddCommentEndpoint(AppDbContext dbContext) : FastEndpoints.Endpoint<AddCommentRequest, ApiResponse<TicketCommentResponse>>
{
    public override void Configure()
    {
        Post("/api/admin/support/tickets/comments");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(AddCommentRequest req, CancellationToken ct)
    {
        var handler = new Handler(dbContext);
        // TODO: Get adminId and adminName from JWT claims
        var result = await handler.AddComment(req, Guid.Empty, "admin", ct);
        await Send.ResponseAsync(ApiResponse<TicketCommentResponse>.SuccessResult(result), cancellation: ct);
    }
}
