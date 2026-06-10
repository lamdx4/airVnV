namespace Airbnb.PropertyService.Features.Admin.Reports.GetStatusFunnel;

public record Request : Mediator.IQuery<Response>;

public record Response(
    int Draft,
    int PendingReview,
    int Published,
    int Suspended,
    int Rejected,
    int Archived
);
