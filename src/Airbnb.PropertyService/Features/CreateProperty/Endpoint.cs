using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.CreateProperty;

/// <summary>
/// Endpoint chỉ làm 1 việc: bridge HTTP → Mediator → Response.
/// Không có business logic. Không biết DB, Domain tồn tại.
/// </summary>
public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/properties");
        Summary(s =>
        {
            s.Summary = "Tạo mới địa điểm lưu trú";
            s.Description = "Command handler chứa toàn bộ logic. Endpoint chỉ dispatch.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
