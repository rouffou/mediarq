using System.Runtime.CompilerServices;
using Mediarq.AotSample;
using Mediarq.Core.Common.Registration;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Extensions;
using Microsoft.Extensions.DependencyInjection;

// Reflection-free, Native-AOT-friendly registration:
//   AddMediarqCore()     registers the core services + built-in behaviors,
//   AddMediarqHandlers() (source-generated) registers the handlers AND pre-populates the strongly-typed
//                        dispatch registry, so Send / Publish / CreateStream never use reflection
//                        (no Activator.CreateInstance, no MakeGenericType, no Expression.Compile).
var services = new ServiceCollection();
services.AddLogging();
services.AddMediarqCore(isHttp: false).AddMediarqHandlers();

using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

// Command returning a Result.
Result<string> ok = await mediator.Send(new Ping("aot"));
Console.WriteLine($"command    : success={ok.IsSuccess}, value={ok.Value}");

// Query returning a Result.
Result<string> greeting = await mediator.Send(new GetGreeting("Ada"));
Console.WriteLine($"query      : {greeting.Value}");

// Validation short-circuits with a Result<string> failure built from the source-generated
// ValidationFailureRegistry (no dynamic code).
Result<string> invalid = await mediator.Send(new Validated(""));
Console.WriteLine($"validation : failure={invalid.IsFailure} ({invalid.Error.Code})");

// Streaming request -> IAsyncEnumerable<int>.
Console.Write("stream     : ");
await foreach (var n in mediator.CreateStream(new Countdown(3)))
{
    Console.Write($"{n} ");
}

Console.WriteLine();

// Notification fanned out to its handler (registered as a singleton via [RegisterHandler]).
await mediator.Publish(new Pinged(42));
Console.WriteLine("done");

namespace Mediarq.AotSample
{
    public record Ping(string Text) : ICommand<Result<string>>;

    public sealed class PingHandler : ICommandHandler<Ping, Result<string>>
    {
        public Task<Result<string>> Handle(Ping request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success(request.Text));
    }

    public record GetGreeting(string Name) : IQuery<Result<string>>;

    public sealed class GetGreetingHandler : IQueryHandler<GetGreeting, Result<string>>
    {
        public Task<Result<string>> Handle(GetGreeting request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success($"Hello, {request.Name}!"));
    }

    public record Validated(string Name) : ICommand<Result<string>>;

    public sealed class ValidatedHandler : ICommandHandler<Validated, Result<string>>
    {
        public Task<Result<string>> Handle(Validated request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success(request.Name));
    }

    public sealed class ValidatedValidator : IValidator<Validated>
    {
        public IEnumerable<ValidationResult> Validate(Validated instance)
        {
            if (string.IsNullOrWhiteSpace(instance.Name))
            {
                yield return ValidationResult.Failure([new ValidationPropertyError("Name", "Name is required.")]);
            }
            else
            {
                yield return ValidationResult.Success();
            }
        }
    }

    public record Countdown(int From) : IStreamRequest<int>;

    public sealed class CountdownHandler : IStreamRequestHandler<Countdown, int>
    {
        public async IAsyncEnumerable<int> Handle(
            Countdown request,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            for (var i = request.From; i >= 1; i--)
            {
                await Task.Yield();
                yield return i;
            }
        }
    }

    public record Pinged(int Id) : INotification;

    // [RegisterHandler] overrides the generated DI lifetime (default is Scoped).
    [RegisterHandler(ServiceLifetime.Singleton)]
    public sealed class PingedHandler : INotificationHandler<Pinged>
    {
        public Task Handle(Pinged notification, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"notified   : {notification.Id}");
            return Task.CompletedTask;
        }
    }
}
