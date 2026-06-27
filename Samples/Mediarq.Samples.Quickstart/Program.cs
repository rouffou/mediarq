using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Extensions;
using Mediarq.Samples.Quickstart.Domain;
using Mediarq.Samples.Quickstart.Orders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// --------------------------------------------------------------------------------------------------
// Mediarq quickstart — a console tour of the core, in-process features (no web, no extensions).
// Each section below dispatches a request and prints the outcome. The interleaved log lines come
// from the pipeline behaviors (audit, request logging, performance) wrapping each handler.
// --------------------------------------------------------------------------------------------------

var services = new ServiceCollection();

services.AddLogging(builder => builder
    .AddSimpleConsole(options => { options.SingleLine = true; options.TimestampFormat = "HH:mm:ss "; })
    .SetMinimumLevel(LogLevel.Information));

services.AddSingleton<IOrderStore, InMemoryOrderStore>();

// Scan this assembly for handlers, validators, behaviors, processors and exception handlers.
// (For trimming / Native AOT, prefer AddMediarqCore().AddMediarqHandlers() — see the AotSample.)
services.AddMediarq(isHttp: false, typeof(Program).Assembly)
        .AddMediarqRequestLogging()      // opt-in LoggingBehavior
        .AddMediarqPerformanceTracking() // opt-in PerformanceBehavior
        .AddMediarqTimeout();            // opt-in TimeoutBehavior (only affects ITimeoutRequest)

using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

// 1. Command returning Result<Guid> — runs validation, pre/post processors, audit and notifications.
Section("1. Command + validation + pre/post + notifications");
var created = await mediator.Send(new CreateOrderCommand("Alice", 42.50m));
Console.WriteLine($"   -> success={created.IsSuccess}, orderId={created.Value}");
var orderId = created.Value;

// 2. Validation short-circuits with a failed Result (the handler never runs).
Section("2. Validation failure (short-circuit)");
var invalid = await mediator.Send(new CreateOrderCommand(Customer: "", Total: -5m));
Console.WriteLine($"   -> success={invalid.IsSuccess}, error=[{invalid.Error.Code}] {invalid.Error.Message}");

// 3. Query — success and typed NotFound failure.
Section("3. Query (found / not found)");
var found = await mediator.Send(new GetOrderByIdQuery(orderId));
Console.WriteLine($"   -> found: {found.Match(o => $"{o.Customer} ({o.Total:0.00})", e => e.Message)}");
var missing = await mediator.Send(new GetOrderByIdQuery(Guid.NewGuid()));
Console.WriteLine($"   -> missing: type={missing.Error.Type}, message={missing.Error.Message}");

// 4. No-result (void) command, dispatched through the same pipeline.
Section("4. Void command");
await mediator.Send(new CancelOrderCommand(orderId));
Console.WriteLine("   -> cancel command dispatched");

// 5. Streaming request consumed with await foreach.
Section("5. Streaming request");
await mediator.Send(new CreateOrderCommand("Bob", 10m));
await mediator.Send(new CreateOrderCommand("Carol", 99m));
await foreach (var order in mediator.CreateStream(new StreamRecentOrders(Count: 5)))
    Console.WriteLine($"   -> streamed {order.Customer} ({order.Total:0.00})");

// 6. Exception handler converts a thrown exception into a failed Result.
Section("6. Exception -> failed Result");
var charged = await mediator.Send(new ChargePaymentCommand(orderId, SimulateGatewayError: true));
Console.WriteLine($"   -> success={charged.IsSuccess}, error=[{charged.Error.Code}] {charged.Error.Message}");

// 7. Timeout: the handler is slower than the request's budget.
Section("7. Timeout (ITimeoutRequest)");
try
{
    await mediator.Send(new GenerateReportCommand());
}
catch (RequestTimeoutException ex)
{
    Console.WriteLine($"   -> timed out as expected: {ex.Message}");
}

// 8. Functional Result combinators (no manual IsSuccess checks).
Section("8. Result combinators");
var message = Result.Success(21)
    .Ensure(x => x > 0, ResultError.Failure("Value.Invalid", "must be positive"))
    .Map(x => x * 2)
    .Match(onSuccess: x => $"value: {x}", onFailure: e => $"error: {e.Message}");
Console.WriteLine($"   -> {message}");

Console.WriteLine();
Console.WriteLine("Done.");

static void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine($"=== {title} ===");
}
