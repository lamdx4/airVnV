namespace Airbnb.SharedKernel.Reports;

public static class BucketStrategyFactory
{
    public static IBucketStrategy For(string? groupBy) => (groupBy ?? "day").ToLowerInvariant() switch
    {
        "month" => new MonthBucketStrategy(),
        "week" => new WeekBucketStrategy(),
        _ => new DayBucketStrategy(),
    };
}
