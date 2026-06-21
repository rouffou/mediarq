using System.Diagnostics;
using FluentAssertions;
using Mediarq.Diagnostics;
using Mediarq.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Mediarq.OpenTelemetry.Tests;

public class MediarqInstrumentationTests
{
    [Fact]
    public void AddMediarqInstrumentation_Tracing_Exports_Mediarq_Activities()
    {
        var exported = new List<Activity>();
        using var provider = Sdk.CreateTracerProviderBuilder()
            .AddMediarqInstrumentation()
            .AddInMemoryExporter(exported)
            .Build();

        using (MediarqDiagnostics.ActivitySource.StartActivity("test-request"))
        {
            // Activity scope: created because the provider subscribes to the Mediarq source.
        }

        provider!.ForceFlush();

        exported.Should().ContainSingle(activity => activity.DisplayName == "test-request");
    }

    [Fact]
    public void AddMediarqInstrumentation_Metrics_Is_Fluent_And_Builds()
    {
        var builder = Sdk.CreateMeterProviderBuilder();

        var returned = builder.AddMediarqInstrumentation();

        returned.Should().BeSameAs(builder);

        using var provider = returned.AddInMemoryExporter(new List<Metric>()).Build();
        provider.Should().NotBeNull();
    }
}
