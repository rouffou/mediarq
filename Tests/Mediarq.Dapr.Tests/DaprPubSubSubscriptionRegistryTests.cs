using FluentAssertions;

namespace Mediarq.Dapr.Tests;

public class DaprPubSubSubscriptionRegistryTests
{
    [Fact]
    public void Add_Appends_To_Subscriptions()
    {
        var registry = new DaprPubSubSubscriptionRegistry();
        var subscription = new DaprSubscription("orders-pubsub", "order-placed", "/dapr/pubsub/OrderPlaced");

        registry.Add(subscription);

        registry.Subscriptions.Should().ContainSingle().Which.Should().Be(subscription);
    }

    [Fact]
    public void Add_Throws_When_Subscription_Is_Null()
    {
        var registry = new DaprPubSubSubscriptionRegistry();

        var act = () => registry.Add(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
