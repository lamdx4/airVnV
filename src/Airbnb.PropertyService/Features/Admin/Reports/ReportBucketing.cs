namespace Airbnb.PropertyService.Features.Admin.Reports;

internal static class ReportBucketing
{
    public static List<(string Key, string Label)> GenerateBuckets(DateOnly from, DateOnly to, string groupBy)
    {
        var list = new List<(string, string)>();
        var cursor = NormalizeStart(from, groupBy);
        while (cursor <= to)
        {
            list.Add((BucketKey(cursor, groupBy), BucketLabel(cursor, groupBy)));
            cursor = Advance(cursor, groupBy);
        }
        return list;
    }

    public static string BucketKey(DateOnly d, string groupBy) => groupBy switch
    {
        "month" => $"{d.Year:D4}-{d.Month:D2}",
        "week" => $"{System.Globalization.ISOWeek.GetYear(d.ToDateTime(TimeOnly.MinValue)):D4}-W{System.Globalization.ISOWeek.GetWeekOfYear(d.ToDateTime(TimeOnly.MinValue)):D2}",
        _ => d.ToString("yyyy-MM-dd")
    };

    private static string BucketLabel(DateOnly d, string groupBy) => groupBy switch
    {
        "month" => d.ToString("yyyy-MM"),
        "week" => $"W{System.Globalization.ISOWeek.GetWeekOfYear(d.ToDateTime(TimeOnly.MinValue)):D2} {d:yyyy}",
        _ => d.ToString("MM-dd")
    };

    private static DateOnly NormalizeStart(DateOnly d, string groupBy) => groupBy switch
    {
        "month" => new DateOnly(d.Year, d.Month, 1),
        "week" => d.AddDays(-((int)d.DayOfWeek == 0 ? 6 : (int)d.DayOfWeek - 1)),
        _ => d
    };

    private static DateOnly Advance(DateOnly d, string groupBy) => groupBy switch
    {
        "month" => d.AddMonths(1),
        "week" => d.AddDays(7),
        _ => d.AddDays(1)
    };
}
