namespace Airbnb.PropertyService.Infrastructure.Messaging;

/// <summary>
/// RabbitMQ exchange/routing key constants cho PropertyService domain events.
/// Dùng string constant thay vì class name để refactor không break consumer.
/// Convention: airbnb.{aggregate}.{event-past-tense}
/// </summary>
public static class PropertyTopics
{
    private const string Prefix = "airbnb.property";

    public const string Submitted      = $"{Prefix}.submitted";
    public const string Published      = $"{Prefix}.published";
    public const string Suspended      = $"{Prefix}.suspended";
    public const string Reinstated     = $"{Prefix}.reinstated";
    public const string Archived       = $"{Prefix}.archived";
    public const string PricingUpdated = $"{Prefix}.pricing-updated";
}
