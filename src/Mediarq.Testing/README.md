# Mediarq.Testing

Test harness for Mediarq: a `SpyMediator` that records every dispatched request/notification while still
running them through the real pipeline (handlers, validators, behaviors — nothing is faked), plus
ready-to-use `FakeClock`/`FakeUserContext`.

```bash
dotnet add package Mediarq.Testing
```

## Spying on dispatch

```csharp
services.AddMediarq(isHttp: false, typeof(Program).Assembly); // your normal registration
services.AddMediarqSpy();                                     // decorates IMediator
```

```csharp
var sender = provider.GetRequiredService<ISender>();
await sender.Send(new CreateOrder(customerId));

var spy = (SpyMediator)provider.GetRequiredService<IMediator>();
spy.HasSent<CreateOrder>().Should().BeTrue();          // any assertion library works
spy.Published<OrderCreated>().Should().ContainSingle();
```

`AddMediarqSpy()` decorates the already-registered `IMediator` — call it **after**
`AddMediarq`/`AddMediarqCore`. `ISender` and `IPublisher` are covered too: both already resolve the
current `IMediator` from the container, so every `Send`/`Publish`/`CreateStream` call is recorded no
matter which of the three interfaces the caller depends on.

In a smaller test, construct one directly instead of going through DI:

```csharp
var spy = new SpyMediator(myRealOrFakeMediator);
await spy.Send(new CreateOrder(customerId));
spy.SentRequests.Should().ContainSingle();
```

## FakeClock / FakeUserContext

```csharp
var clock = new FakeClock { UtcNow = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
var user = new FakeUserContext { UserId = "u1", UserName = "alice", Roles = ["Admin"] };

services.AddSingleton<IClock>(clock);
services.AddScoped<IUserContext>(_ => user);
```

Both are plain settable implementations of `IClock`/`IUserContext` — register them in place of
`SystemClock`/`HttpUserContext`/`DefaultUserContext` in a test's `IServiceCollection`.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
