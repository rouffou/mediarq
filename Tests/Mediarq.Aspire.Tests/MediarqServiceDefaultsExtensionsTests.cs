using System.Diagnostics;
using FluentAssertions;
using Mediarq.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Mediarq.Aspire.Tests;

public class MediarqServiceDefaultsExtensionsTests
{
    [Fact]
    public async Task AddMediarqServiceDefaults_Wires_Tracing_For_The_Mediarq_ActivitySource()
    {
        var exported = new List<Activity>();
        var builder = Host.CreateApplicationBuilder();

        builder.AddMediarqServiceDefaults(typeof(MediarqServiceDefaultsExtensionsTests).Assembly);
        builder.Services.AddOpenTelemetry().WithTracing(t => t.AddInMemoryExporter(exported));

        using var host = builder.Build();
        await host.StartAsync();
        try
        {
            using (MediarqDiagnostics.ActivitySource.StartActivity("test-request"))
            {
                // Activity scope: created because the provider subscribes to the Mediarq source.
            }

            host.Services.GetRequiredService<TracerProvider>().ForceFlush();

            exported.Should().ContainSingle(a => a.DisplayName == "test-request");
        }
        finally
        {
            await host.StopAsync();
        }
    }

    [Fact]
    public async Task AddMediarqServiceDefaults_Registers_The_Handler_Registration_Health_Check()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.AddMediarqServiceDefaults(typeof(MediarqServiceDefaultsExtensionsTests).Assembly);

        using var host = builder.Build();
        var healthCheckService = host.Services.GetRequiredService<HealthCheckService>();

        var report = await healthCheckService.CheckHealthAsync();

        report.Entries.Should().ContainKey("mediarq_handlers");
    }

    [Fact]
    public async Task AddMediarqServiceDefaults_Is_Composable_With_An_Existing_AddOpenTelemetry_Call()
    {
        var exported = new List<Activity>();
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddOpenTelemetry().WithTracing(t => t.AddSource("SomeOtherApp.Source"));

        builder.AddMediarqServiceDefaults();
        builder.Services.AddOpenTelemetry().WithTracing(t => t.AddInMemoryExporter(exported));

        using var host = builder.Build();
        await host.StartAsync();
        try
        {
            using (MediarqDiagnostics.ActivitySource.StartActivity("test-request"))
            {
            }

            host.Services.GetRequiredService<TracerProvider>().ForceFlush();

            exported.Should().ContainSingle(a => a.DisplayName == "test-request");
        }
        finally
        {
            await host.StopAsync();
        }
    }

    [Fact]
    public void AddMediarqServiceDefaults_Returns_The_Same_Builder()
    {
        var builder = Host.CreateApplicationBuilder();

        var result = builder.AddMediarqServiceDefaults();

        result.Should().BeSameAs(builder);
    }

    [Fact]
    public void AddMediarqServiceDefaults_Throws_When_Builder_Is_Null()
    {
        IHostApplicationBuilder builder = null!;

        var act = () => builder.AddMediarqServiceDefaults();

        act.Should().Throw<ArgumentNullException>();
    }
}
