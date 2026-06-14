using Mediarq.AotSample;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Extensions;
using Microsoft.Extensions.DependencyInjection;

// Reflection-free, Native AOT friendly registration:
//   AddMediarqCore() registers the core services + built-in behaviors,
//   AddMediarqHandlers() (source-generated) registers the handlers AND pre-populates the
//   strongly-typed dispatch registry, so Send/Publish never use Activator.CreateInstance.
var services = new ServiceCollection();
services.AddLogging();
services.AddMediarqCore(isHttp: false).AddMediarqHandlers();

using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

Result<string> ok = await mediator.Send(new Ping("aot"));
Console.WriteLine($"send       : success={ok.IsSuccess}, value={ok.Value}");

// Goes through ValidationBehavior, which short-circuits with a Result<string> failure built from the
// source-generated ValidationFailureRegistry (no Expression.Compile / dynamic code).
Result<string> invalid = await mediator.Send(new Validated(""));
Console.WriteLine($"validation : failure={invalid.IsFailure}");

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

    public record Pinged(int Id) : INotification;

    public sealed class PingedHandler : INotificationHandler<Pinged>
    {
        public Task Handle(Pinged notification, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"notified   : {notification.Id}");
            return Task.CompletedTask;
        }
    }
}
