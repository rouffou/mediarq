using FluentAssertions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Saga;

namespace Mediarq.Saga.Tests;

public class SagaNotificationHandlerTests
{
    private sealed class OrderState : ISagaState
    {
        public required Guid CorrelationId { get; init; }
        public bool IsComplete { get; set; }
        public bool OrderPlaced { get; set; }
        public bool PaymentReceived { get; set; }
    }

    private sealed record OrderPlaced(Guid OrderId) : INotification;

    private sealed record PaymentReceived(Guid OrderId) : INotification;

    private sealed class OnOrderPlaced(ISagaStore<OrderState> store, int[] createCount)
        : SagaNotificationHandler<OrderPlaced, OrderState>(store)
    {
        protected override Guid GetCorrelationId(OrderPlaced notification) => notification.OrderId;

        protected override OrderState CreateState(Guid correlationId)
        {
            createCount[0]++;
            return new OrderState { CorrelationId = correlationId };
        }

        protected override Task HandleAsync(OrderPlaced notification, OrderState state, CancellationToken cancellationToken)
        {
            state.OrderPlaced = true;
            return Task.CompletedTask;
        }
    }

    private sealed class OnPaymentReceived(ISagaStore<OrderState> store, int[] handleCount)
        : SagaNotificationHandler<PaymentReceived, OrderState>(store)
    {
        protected override Guid GetCorrelationId(PaymentReceived notification) => notification.OrderId;

        protected override OrderState CreateState(Guid correlationId) => new() { CorrelationId = correlationId };

        protected override Task HandleAsync(PaymentReceived notification, OrderState state, CancellationToken cancellationToken)
        {
            handleCount[0]++;
            state.PaymentReceived = true;
            state.IsComplete = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Two_Steps_Share_State_Through_The_Same_Correlation_Id()
    {
        var store = new InMemorySagaStore<OrderState>();
        var orderId = Guid.NewGuid();
        var createCount = new[] { 0 };

        await new OnOrderPlaced(store, createCount).Handle(new OrderPlaced(orderId));
        await new OnPaymentReceived(store, [0]).Handle(new PaymentReceived(orderId));

        var state = await store.FindAsync(orderId);
        state.Should().NotBeNull();
        state!.OrderPlaced.Should().BeTrue();
        state.PaymentReceived.Should().BeTrue();
        state.IsComplete.Should().BeTrue();
    }

    [Fact]
    public async Task CreateState_Runs_Only_Once_Per_Correlation_Id()
    {
        var store = new InMemorySagaStore<OrderState>();
        var orderId = Guid.NewGuid();
        var createCount = new[] { 0 };
        var handler = new OnOrderPlaced(store, createCount);

        await handler.Handle(new OrderPlaced(orderId));
        await handler.Handle(new OrderPlaced(orderId));

        createCount[0].Should().Be(1);
    }

    [Fact]
    public async Task Notifications_For_Different_Correlation_Ids_Get_Independent_State()
    {
        var store = new InMemorySagaStore<OrderState>();
        var handler = new OnOrderPlaced(store, [0]);
        var firstOrder = Guid.NewGuid();
        var secondOrder = Guid.NewGuid();

        await handler.Handle(new OrderPlaced(firstOrder));

        (await store.FindAsync(firstOrder)).Should().NotBeNull();
        (await store.FindAsync(secondOrder)).Should().BeNull();
    }

    [Fact]
    public async Task A_Completed_Instance_Ignores_Further_Notifications()
    {
        var store = new InMemorySagaStore<OrderState>();
        var orderId = Guid.NewGuid();
        var handleCount = new[] { 0 };
        var handler = new OnPaymentReceived(store, handleCount);

        await handler.Handle(new PaymentReceived(orderId));
        await handler.Handle(new PaymentReceived(orderId));

        handleCount[0].Should().Be(1, "the second call should be skipped because the instance is already complete");
    }

    [Fact]
    public async Task Handle_Throws_For_A_Null_Notification()
    {
        var store = new InMemorySagaStore<OrderState>();
        var handler = new OnOrderPlaced(store, [0]);

        var act = () => handler.Handle(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
