namespace Airbnb.SharedKernel.Reports;

public sealed class DayBucketStrategy : IBucketStrategy
{
    public string Key(DateOnly date) => date.ToString("yyyy-MM-dd");
    public string Label(DateOnly date) => date.ToString("MM-dd");
    public DateOnly NormalizeStart(DateOnly date) => date;
    public DateOnly Advance(DateOnly date) => date.AddDays(1);
}
