namespace Airbnb.PropertyService.Features.Admin.Reports.GetPendingBacklog;

public record Request : Mediator.IQuery<Response>;

public record Response(
    int PendingCount,
    double AverageWaitDays,
    double MaxWaitDays,
    int OverdueCount,
    int OverdueThresholdDays
);
