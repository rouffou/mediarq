using System.Diagnostics;
using System.Diagnostics.Metrics;
using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Common.Results;
using Mediarq.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Diagnostics.Tests;

public class DiagnosticsBehaviorTests
{
    public record PingQuery : IQuery<Result<string>>;

    [Fact]
    public async Task Emits_Activity_For_Request()
    {
        var stopped = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == MediarqDiagnostics.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = stopped.Add,
        };
        ActivitySource.AddActivityListener(listener);

        var behavior = new DiagnosticsBehavior<PingQuery, Result<string>>();
        var context = new RequestContext<PingQuery, Result<string>>(new PingQuery(), "user");

        await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")));

        stopped.Should().ContainSingle(a => a.OperationName == "Mediarq:PingQuery");
    }

    [Fact]
    public async Task Records_Request_Count_Metric()
    {
        long count = 0;
        using var meterListener = new MeterListener();
        meterListener.InstrumentPublished = (instrument, l) =>
        {
            if (instrument.Meter.Name == MediarqDiagnostics.SourceName && instrument.Name == "mediarq.requests.count")
            {
                l.EnableMeasurementEvents(instrument);
            }
        };
        meterListener.SetMeasurementEventCallback<long>((_, measurement, _, _) => Interlocked.Add(ref count, measurement));
        meterListener.Start();

        var behavior = new DiagnosticsBehavior<PingQuery, Result<string>>();
        var context = new RequestContext<PingQuery, Result<string>>(new PingQuery(), "user");

        await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")));

        count.Should().Be(1);
    }

    [Fact]
    public async Task Emits_Messaging_SemanticConvention_Tags_For_Request()
    {
        var stopped = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == MediarqDiagnostics.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = stopped.Add,
        };
        ActivitySource.AddActivityListener(listener);

        var behavior = new DiagnosticsBehavior<PingQuery, Result<string>>();
        var context = new RequestContext<PingQuery, Result<string>>(new PingQuery(), "user");

        await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")));

        var activity = stopped.Should().ContainSingle(a => a.OperationName == "Mediarq:PingQuery").Subject;
        activity.GetTagItem("messaging.system").Should().Be(MediarqDiagnostics.MessagingSystem);
        activity.GetTagItem("messaging.operation.type").Should().Be("process");
        activity.GetTagItem("messaging.destination.name").Should().Be(nameof(PingQuery));
        activity.GetTagItem("messaging.message.id").Should().Be(context.RequestId.ToString());
    }

    [Fact]
    public async Task Emits_ErrorType_Tag_When_Request_Throws()
    {
        var stopped = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == MediarqDiagnostics.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = stopped.Add,
        };
        ActivitySource.AddActivityListener(listener);

        var behavior = new DiagnosticsBehavior<PingQuery, Result<string>>();
        var context = new RequestContext<PingQuery, Result<string>>(new PingQuery(), "user");

        var act = () => behavior.Handle(context, Task<Result<string>> () => throw new InvalidOperationException("boom"));

        await act.Should().ThrowAsync<InvalidOperationException>();
        var activity = stopped.Should().ContainSingle().Subject;
        activity.GetTagItem("error.type").Should().Be(typeof(InvalidOperationException).FullName);
    }

    [Fact]
    public async Task Records_Messaging_Operation_Duration_Metric_For_Request()
    {
        var measurements = 0;
        using var meterListener = new MeterListener();
        meterListener.InstrumentPublished = (instrument, l) =>
        {
            if (instrument.Meter.Name == MediarqDiagnostics.SourceName && instrument.Name == "messaging.client.operation.duration")
            {
                l.EnableMeasurementEvents(instrument);
            }
        };
        meterListener.SetMeasurementEventCallback<double>((_, _, _, _) => Interlocked.Increment(ref measurements));
        meterListener.Start();

        var behavior = new DiagnosticsBehavior<PingQuery, Result<string>>();
        var context = new RequestContext<PingQuery, Result<string>>(new PingQuery(), "user");

        await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")));

        measurements.Should().Be(1);
    }

    [Fact]
    public void AddMediarqDiagnostics_Registers_Behaviors()
    {
        var services = new ServiceCollection();

        services.AddMediarqDiagnostics();

        services.Should().Contain(d => d.ServiceType == typeof(IPipelineBehavior<,>)
            && d.ImplementationType == typeof(DiagnosticsBehavior<,>));
        services.Should().Contain(d => d.ServiceType == typeof(IStreamPipelineBehavior<,>)
            && d.ImplementationType == typeof(StreamDiagnosticsBehavior<,>));
    }

    [Fact]
    public async Task Emits_Activity_For_Stream()
    {
        var stopped = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == MediarqDiagnostics.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = stopped.Add,
        };
        ActivitySource.AddActivityListener(listener);

        var behavior = new StreamDiagnosticsBehavior<TickStream, int>();

        var items = new List<int>();
        await foreach (var item in behavior.Handle(new TickStream(), Ticks, CancellationToken.None))
        {
            items.Add(item);
        }

        items.Should().Equal(0, 1, 2);
        var activity = stopped.Should().ContainSingle(a => a.OperationName == "Mediarq:Stream TickStream").Subject;
        activity.GetTagItem("messaging.system").Should().Be(MediarqDiagnostics.MessagingSystem);
        activity.GetTagItem("messaging.operation.type").Should().Be("process");
        activity.GetTagItem("messaging.destination.name").Should().Be(nameof(TickStream));
        activity.GetTagItem("messaging.batch.message_count").Should().Be(3L);
    }

    [Fact]
    public async Task Notification_Publisher_Emits_Activity_And_Invokes_Handlers()
    {
        var stopped = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == MediarqDiagnostics.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = stopped.Add,
        };
        ActivitySource.AddActivityListener(listener);

        var publisher = new DiagnosticsNotificationPublisher(new ParallelNotificationPublisher());
        var invoked = false;
        var handlers = new List<Func<CancellationToken, Task>> { _ => { invoked = true; return Task.CompletedTask; } };

        await publisher.Publish(handlers, CancellationToken.None);

        invoked.Should().BeTrue();
        var activity = stopped.Should().ContainSingle(a => a.OperationName == "Mediarq:Publish").Subject;
        activity.GetTagItem("messaging.system").Should().Be(MediarqDiagnostics.MessagingSystem);
        activity.GetTagItem("messaging.operation.type").Should().Be("publish");
        activity.GetTagItem("messaging.batch.message_count").Should().Be(1L);
    }

    [Fact]
    public void AddMediarqDiagnostics_Decorates_Notification_Publisher()
    {
        var services = new ServiceCollection();
        services.AddSingleton<INotificationPublisher, ParallelNotificationPublisher>();

        services.AddMediarqDiagnostics();

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<INotificationPublisher>().Should().BeOfType<DiagnosticsNotificationPublisher>();
    }

    public record TickStream : IStreamRequest<int>;

    private static async IAsyncEnumerable<int> Ticks()
    {
        for (var i = 0; i < 3; i++)
        {
            yield return i;
            await Task.Yield();
        }
    }
}
