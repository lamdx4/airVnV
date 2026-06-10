namespace Airbnb.UserService.Features.Admin.GetUserActivityReport;

public record Request : Mediator.IQuery<Response>;

public record Response(
    int ActiveLast7Days,
    int ActiveLast30Days,
    int ActiveLast90Days,
    int InactiveOver90Days,
    int NeverLoggedIn
);
