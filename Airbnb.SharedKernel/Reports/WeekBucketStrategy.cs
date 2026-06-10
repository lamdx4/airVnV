using System.Globalization;

namespace Airbnb.SharedKernel.Reports;

public sealed class WeekBucketStrategy : IBucketStrategy
{
    public string Key(DateOnly date)
    {
        var dt = date.ToDateTime(TimeOnly.MinValue);
        return $"{ISOWeek.GetYear(dt):D4}-W{ISOWeek.GetWeekOfYear(dt):D2}";
    }

    public string Label(DateOnly date)
    {
        var dt = date.ToDateTime(TimeOnly.MinValue);
        return $"W{ISOWeek.GetWeekOfYear(dt):D2} {date:yyyy}";
    }

    public DateOnly NormalizeStart(DateOnly date)
    {
        var offset = (int)date.DayOfWeek == 0 ? 6 : (int)date.DayOfWeek - 1;
        return date.AddDays(-offset);
    }

    public DateOnly Advance(DateOnly date) => date.AddDays(7);
}
