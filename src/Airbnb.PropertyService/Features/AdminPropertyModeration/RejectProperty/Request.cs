using Mediator;

namespace Airbnb.PropertyService.Features.AdminPropertyModeration.RejectProperty;

public record Request(
    Guid PropertyId,
    string? Reason = null
) : ICommand<Response>;
