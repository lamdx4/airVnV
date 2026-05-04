using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageAvailability.RemoveAvailability;

public record Request(Guid PropertyId, Guid AvailabilityId, Guid RequesterId) 
    : Mediator.ICommand<ApiResponse<bool>>;
