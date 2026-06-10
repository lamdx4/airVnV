namespace Airbnb.PaymentService.Features.Admin.BootstrapHostBalances;

public record Request : Mediator.ICommand<Response>;

public record Response(
    int EntriesCreated,
    int BalancesUpdated,
    int PayoutsRebuilt,
    string Message
);
