using System.Reflection;
using Airbnb.SharedKernel.Domain;
using Airbnb.PropertyService.Infrastructure.Messaging;

namespace Airbnb.PropertyService.Tests.Infrastructure;

/// <summary>
/// Exhaustiveness test: verify mọi IDomainEvent implementation đều có trong TopicMap.
/// Test này fail CI ngay nếu ai thêm event mới mà quên thêm vào TopicMap.
/// </summary>
public class DomainEventPublisherTests
{
    [Fact]
    public void TopicMap_MustCoverAllDomainEventImplementations()
    {
        // Scan PropertyService assembly – nơi domain events được định nghĩa
        // Không dùng typeof(IDomainEvent).Assembly vì IDomainEvent giờ ở SharedKernel
        var propertyServiceAssembly = typeof(Airbnb.PropertyService.Domain.PropertyPublishedEvent).Assembly;

        var allEventTypes = propertyServiceAssembly
            .GetTypes()
            .Where(t =>
                typeof(IDomainEvent).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract)
            .ToList();

        // Assert: phải có ít nhất 1 event (tránh false positive nếu assembly rỗng)
        Assert.NotEmpty(allEventTypes);

        // Truy cập trực tiếp qua InternalsVisibleTo – không cần reflection
        var missing = allEventTypes
            .Where(t => !DomainEventPublisher.TopicMap.ContainsKey(t))
            .Select(t => t.Name)
            .OrderBy(n => n)
            .ToList();

        Assert.True(
            missing.Count == 0,
            $"The following events are missing from DomainEventPublisher.TopicMap:\n"
            + string.Join("\n", missing.Select(n => $"  - {n}")));
    }

    [Fact]
    public void TopicMap_AllTopicsMustHaveCorrectPrefix()
    {
        const string expectedPrefix = "airbnb.property.";

        var invalidTopics = DomainEventPublisher.TopicMap.Values
            .Where(topic => !topic.StartsWith(expectedPrefix))
            .ToList();

        Assert.True(
            invalidTopics.Count == 0,
            $"The following topics don't follow naming convention '{expectedPrefix}...':\n"
            + string.Join("\n", invalidTopics.Select(t => $"  - {t}")));
    }
}
