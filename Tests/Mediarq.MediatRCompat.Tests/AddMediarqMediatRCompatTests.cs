using FluentAssertions;
using Mediarq.Extensions;
using Mediarq.MediatRCompat.Tests.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.MediatRCompat.Tests;

public class AddMediarqMediatRCompatTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<ExecutionTrace>();
        services.AddMediarqCore(isHttp: false);
        services.AddMediarqMediatRCompat(typeof(Ping).Assembly);
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Send_Dispatches_A_MediatR_Request_With_Response_Through_Its_Original_Handler()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();

        var response = await mediator.Send(new Ping("hi"));

        response.Should().Be("hi");
    }

    [Fact]
    public async Task Send_Dispatches_A_Void_MediatR_Request_Through_Its_Original_Handler()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var trace = scope.ServiceProvider.GetRequiredService<ExecutionTrace>();

        await mediator.Send(new TracedVoidCommand());

        trace.Entries.Should().Contain("handler");
    }

    [Fact]
    public async Task Send_Object_Dynamically_Dispatches_A_Request_With_Response()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();

        object request = new Ping("dynamic");
        var response = await mediator.Send(request);

        response.Should().Be("dynamic");
    }

    [Fact]
    public async Task Send_Object_Dynamically_Dispatches_A_Void_Request()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var trace = scope.ServiceProvider.GetRequiredService<ExecutionTrace>();

        object request = new TracedVoidCommand();
        await mediator.Send(request);

        trace.Entries.Should().Contain("handler");
    }

    [Fact]
    public async Task Send_Object_Throws_When_The_Request_Is_Not_MediatR_Shaped()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();

        var act = () => mediator.Send(new object());

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Publish_Fans_Out_To_All_Registered_Notification_Handlers()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var trace = scope.ServiceProvider.GetRequiredService<ExecutionTrace>();

        await mediator.Publish(new OrderPlaced(42));

        trace.Entries.Should().Contain("audit:42");
        trace.Entries.Should().Contain("email:42");
    }

    [Fact]
    public async Task Publish_Object_Dynamically_Fans_Out_To_All_Registered_Notification_Handlers()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var trace = scope.ServiceProvider.GetRequiredService<ExecutionTrace>();

        object notification = new OrderPlaced(7);
        await mediator.Publish(notification);

        trace.Entries.Should().Contain("audit:7");
        trace.Entries.Should().Contain("email:7");
    }

    [Fact]
    public async Task CreateStream_Streams_Items_Through_The_Original_Handler()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();

        var items = new List<int>();
        await foreach (var item in mediator.CreateStream(new CountStream(3)))
        {
            items.Add(item);
        }

        items.Should().Equal(1, 2, 3);
    }

    [Fact]
    public async Task CreateStream_Object_Dynamically_Streams_Items()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();

        object request = new CountStream(2);
        var items = new List<object?>();
        await foreach (var item in mediator.CreateStream(request))
        {
            items.Add(item);
        }

        items.Should().Equal(1, 2);
    }

    [Fact]
    public async Task Resolving_ISender_And_IPublisher_Directly_Uses_The_Same_Compat_Mediator()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<MediatR.ISender>();
        var publisher = scope.ServiceProvider.GetRequiredService<MediatR.IPublisher>();
        var trace = scope.ServiceProvider.GetRequiredService<ExecutionTrace>();

        var response = await sender.Send(new Ping("split"));
        await publisher.Publish(new OrderPlaced(1));

        response.Should().Be("split");
        trace.Entries.Should().Contain("audit:1");
    }
}
