namespace Airbnb.SharedKernel.Reports;

public interface IBucketStrategy
{
    string Key(DateOnly date);
    string Label(DateOnly date);
    DateOnly NormalizeStart(DateOnly date);
    DateOnly Advance(DateOnly date);

    List<(string Key, string Label)> GenerateBuckets(DateOnly from, DateOnly to)
    {
        var list = new List<(string, string)>();
        var cursor = NormalizeStart(from);
        while (cursor <= to)
        {
            list.Add((Key(cursor), Label(cursor)));
            cursor = Advance(cursor);
        }
        return list;
    }
}
