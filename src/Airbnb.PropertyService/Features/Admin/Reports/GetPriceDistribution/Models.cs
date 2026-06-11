namespace Airbnb.PropertyService.Features.Admin.Reports.GetPriceDistribution;

public record Request : Mediator.IQuery<Response>;

public record PriceBucket(string Label, decimal Min, decimal? Max, int Count);

public record Response(
    List<PriceBucket> Buckets,
    decimal Median,
    decimal P90,
    decimal Average,
    int Total
);
