using FluentAssertions;
using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Tests.Core.Common.Requests.Notifications;

public class NotificationPublisherTests
{
    [Fact]
    public async Task Parallel_Invokes_All_Handlers()
    {
        var count = 0;
        var handlers = new List<Func<CancellationToken, Task>>
        {
            _ => { Interlocked.Increment(ref count); return Task.CompletedTask; },
            _ => { Interlocked.Increment(ref count); return Task.CompletedTask; },
            _ => { Interlocked.Increment(ref count); return Task.CompletedTask; },
        };

        await new ParallelNotificationPublisher().Publish(handlers, CancellationToken.None);

        count.Should().Be(3);
    }

    [Fact]
    public async Task Sequential_Invokes_All_Handlers_In_Order()
    {
        var order = new List<int>();
        var handlers = new List<Func<CancellationToken, Task>>
        {
            _ => { order.Add(1); return Task.CompletedTask; },
            _ => { order.Add(2); return Task.CompletedTask; },
            _ => { order.Add(3); return Task.CompletedTask; },
        };

        await new SequentialNotificationPublisher().Publish(handlers, CancellationToken.None);

        order.Should().Equal(1, 2, 3);
    }

    [Fact]
    public async Task Publishers_Throw_When_Handlers_Null()
    {
        await FluentActions.Invoking(() => new ParallelNotificationPublisher().Publish(null!, CancellationToken.None))
            .Should().ThrowAsync<ArgumentNullException>();

        await FluentActions.Invoking(() => new SequentialNotificationPublisher().Publish(null!, CancellationToken.None))
            .Should().ThrowAsync<ArgumentNullException>();
    }
}
