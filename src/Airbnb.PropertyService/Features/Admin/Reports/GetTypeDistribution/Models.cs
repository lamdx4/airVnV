namespace Airbnb.PropertyService.Features.Admin.Reports.GetTypeDistribution;

public record Request : Mediator.IQuery<List<TypeCount>>;

public record TypeCount(string Type, int Count);
