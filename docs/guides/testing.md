# Testing

Mediarq handlers, validators and behaviors are plain classes resolved from DI, so they're easy to test
at two levels: **unit** (the class in isolation) and **integration** (through the real mediator).

## Unit-test a handler

A handler has no Mediarq dependency — construct it with test doubles and call `Handle` directly. Assert
on the returned `Result`.

```csharp
[Fact]
public async Task GetOrder_returns_NotFound_when_missing()
{
    var store = new InMemoryOrderStore();              // a fake, or a mock
    var handler = new GetOrderByIdHandler(store);

    var result = await handler.Handle(new GetOrderByIdQuery(Guid.NewGuid()));

    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.NotFound);
}

[Fact]
public async Task CreateOrder_publishes_OrderPlaced()
{
    var publisher = new Mock<IPublisher>();
    var handler = new CreateOrderHandler(new InMemoryOrderStore(), publisher.Object);

    var result = await handler.Handle(new CreateOrderCommand("Alice", 10m));

    result.IsSuccess.Should().BeTrue();
    publisher.Verify(p => p.Publish(It.IsAny<OrderPlaced>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

This is the fast path — no container, no pipeline. Use it for handler logic and the success/failure
branches.

## Unit-test a validator

A built-in `IValidator<T>` is just a method:

```csharp
[Fact]
public void CreateOrder_requires_a_customer()
{
    var results = new CreateOrderCommandValidator().Validate(new CreateOrderCommand("", -1m)).ToList();

    results.Should().ContainSingle();
    results[0].IsValid.Should().BeFalse();
}
```

## Unit-test a behavior

A behavior receives a `context` and a `handle` delegate — call it with a stub `handle`:

```csharp
[Fact]
public async Task AuditBehavior_calls_through_to_the_handler()
{
    var behavior = new AuditBehavior<Ping, Result>(NullLogger<AuditBehavior<Ping, Result>>.Instance);
    var context  = /* build an IMutableRequestContext<Ping, Result> via RequestContextFactory, or a stub */;

    var response = await behavior.Handle(context, () => Task.FromResult(Result.Success()));

    response.IsSuccess.Should().BeTrue();
}
```

For most behaviors the integration test below is simpler than constructing a context by hand.

## Integration-test through the mediator

To exercise the **whole pipeline** (validation, behaviors, your handler), register Mediarq in a
`ServiceCollection` and resolve `IMediator` — exactly like your app does.

```csharp
[Fact]
public async Task Pipeline_short_circuits_on_invalid_command()
{
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddSingleton<IOrderStore, InMemoryOrderStore>();
    services.AddMediarq(isHttp: false, typeof(CreateOrderCommand).Assembly);

    using var provider = services.BuildServiceProvider();
    var mediator = provider.GetRequiredService<IMediator>();

    var result = await mediator.Send(new CreateOrderCommand("", -1m)); // invalid

    result.IsFailure.Should().BeTrue();                 // ValidationBehavior short-circuited
    result.Error.Type.Should().Be(ErrorType.Validation);
}
```

> Register validators/adapters in the same order as your app — see
> [Wiring extensions](wiring-extensions.md). Otherwise the validation behavior may not be wired and your
> "invalid" assertion will fail.

## Integration-test an ASP.NET Core app

Use `WebApplicationFactory<Program>` against your real `Program.cs` (expose it with
`public partial class Program;` at the end of the file) and hit the endpoints:

```csharp
public class OrdersApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_then_get_roundtrips()
    {
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/orders", new { customer = "Alice", items = new[] { new { product = "Widget", quantity = 1, unitPrice = 9.5 } } });
        created.StatusCode.Should().Be(HttpStatusCode.OK);

        var id = await created.Content.ReadFromJsonAsync<Guid>();
        var got = await client.GetAsync($"/orders/{id}");
        got.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

An in-memory EF Core provider (as in the WebApi sample) keeps these tests fast and isolated.

## What to test where

| Level | Good for | Cost |
|---|---|---|
| Unit (handler/validator/behavior) | branching logic, error mapping, edge cases | cheap, fast |
| Integration (mediator) | validation + behaviors + handler together | medium |
| Integration (WebApplicationFactory) | routing, `Result` → HTTP, idempotency, the real wiring | slower |

The library's own test suite (`Tests/`) is a worked reference for all three.
