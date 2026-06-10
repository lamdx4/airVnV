namespace Airbnb.SharedKernel.Reports;

public sealed class MonthBucketStrategy : IBucketStrategy
{
    public string Key(DateOnly date) => $"{date.Year:D4}-{date.Month:D2}";
    public string Label(DateOnly date) => date.ToString("yyyy-MM");
    public DateOnly NormalizeStart(DateOnly date) => new(date.Year, date.Month, 1);
    public DateOnly Advance(DateOnly date) => date.AddMonths(1);
}
