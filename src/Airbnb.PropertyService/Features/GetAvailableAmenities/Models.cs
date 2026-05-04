using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetAvailableAmenities;

public record Request : Mediator.IQuery<ApiResponse<List<AmenityResponse>>>;

public record AmenityResponse(Guid Id, string Name, string Category, string? IconCode);
