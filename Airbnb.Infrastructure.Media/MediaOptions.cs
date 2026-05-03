namespace Airbnb.Infrastructure.Media;

public class MediaOptions
{
    public const string SectionName = "Media";
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}
