using FluentAssertions;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Extensions;
using Mediarq.Tests.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Tests.Extensions;

public class AddMediarqTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<ExecutionTrace>();
        // Pass the test assembly explicitly (exercises the configurable scanner overload).
        services.AddMediarq(isHttp: false, typeof(PingCommand).Assembly);
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Resolves_Mediator_And_Handles_Command_With_Result()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new PingCommand("hi"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hi");
    }

    [Fact]
    public async Task Routes_Void_Command_Through_Pipeline_To_Handler()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var trace = scope.ServiceProvider.GetRequiredService<ExecutionTrace>();

        await mediator.Send(new TracedVoidCommand());

        // The void command reached its handler AND went through the pipeline behaviors.
        trace.Entries.Should().Contain("handler");
        trace.Entries.Should().Contain("before:TracedVoidCommand");
        trace.Entries.Should().Contain("after:TracedVoidCommand");
    }

    [Fact]
    public async Task Validation_Behavior_ShortCircuits_Invalid_Command()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new ValidatedCommand(""));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
    }

    [Fact]
    public async Task Validation_Behavior_Passes_Valid_Command()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new ValidatedCommand("ok"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public async Task Publishes_Notification_To_All_Registered_Handlers()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var trace = scope.ServiceProvider.GetRequiredService<ExecutionTrace>();

        await mediator.Publish(new OrderPlaced(42));

        trace.Entries.Should().Contain("audit:42");
        trace.Entries.Should().Contain("email:42");
    }

    // --- Compile-time generated registration path (AddMediarqCore + generated AddMediarqHandlers) ---

    private static ServiceProvider BuildGeneratedProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<ExecutionTrace>();
        services.AddMediarqCore(isHttp: false);
        services.AddMediarqHandlers(); // generated at compile time by Mediarq.SourceGenerators
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Generated_Registration_Resolves_Mediator_And_Handles_Command()
    {
        using var provider = BuildGeneratedProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new PingCommand("gen"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("gen");
    }

    [Fact]
    public async Task Generated_Registration_Routes_Void_Command_Through_Pipeline()
    {
        using var provider = BuildGeneratedProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var trace = scope.ServiceProvider.GetRequiredService<ExecutionTrace>();

        await mediator.Send(new TracedVoidCommand());

        trace.Entries.Should().Contain("handler");
        // The open-generic TraceBehavior<,> was registered by the source generator.
        trace.Entries.Should().Contain("before:TracedVoidCommand");
    }

    [Fact]
    public async Task Generated_Registration_Applies_Validators()
    {
        using var provider = BuildGeneratedProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new ValidatedCommand(""));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
    }
}
