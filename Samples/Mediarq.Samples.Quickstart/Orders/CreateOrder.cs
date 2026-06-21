using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Processors;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Samples.Quickstart.Domain;
using Microsoft.Extensions.Logging;

namespace Mediarq.Samples.Quickstart.Orders;

/// <summary>A command that returns a value: the new order id wrapped in a <see cref="Result{T}"/>.</summary>
public sealed record CreateOrderCommand(string Customer, decimal Total) : ICommand<Result<Guid>>;

/// <summary>
/// Persists the order and publishes an <see cref="OrderPlaced"/> notification. Validation, the audit
/// behavior and the pre/post processors all run around this handler through the Mediarq pipeline.
/// </summary>
public sealed class CreateOrderCommandHandler(IOrderStore store, IPublisher publisher)
    : ICommandHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken = default)
    {
        var order = new Order { Id = Guid.NewGuid(), Customer = request.Customer, Total = request.Total };
        store.Add(order);

        // Fan-out to every INotificationHandler<OrderPlaced> (runs concurrently by default).
        await publisher.Publish(new OrderPlaced(order.Id, order.Customer, order.Total), cancellationToken);

        return Result.Success(order.Id);
    }
}

/// <summary>
/// Built-in validation: implement <see cref="IValidator{T}"/> and the ValidationBehavior runs it before
/// the handler, short-circuiting with a failed <see cref="Result{T}"/> when the request is invalid.
/// </summary>
public sealed class CreateOrderCommandValidator : IValidator<CreateOrderCommand>
{
    public IEnumerable<ValidationResult> Validate(CreateOrderCommand instance)
    {
        var errors = new List<ValidationPropertyError>();

        if (string.IsNullOrWhiteSpace(instance.Customer))
            errors.Add(new ValidationPropertyError(nameof(instance.Customer), "Customer is required."));

        if (instance.Total <= 0)
            errors.Add(new ValidationPropertyError(nameof(instance.Total), "Total must be greater than zero."));

        yield return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}

/// <summary>Runs before the handler — e.g. enrich, log or normalize the request.</summary>
public sealed class CreateOrderPreProcessor(ILogger<CreateOrderPreProcessor> logger)
    : IRequestPreProcessor<CreateOrderCommand>
{
    public Task Process(CreateOrderCommand request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[pre]  about to create an order for {Customer}", request.Customer);
        return Task.CompletedTask;
    }
}

/// <summary>Runs after the handler — e.g. metrics, auditing, outbox flush.</summary>
public sealed class CreateOrderPostProcessor(ILogger<CreateOrderPostProcessor> logger)
    : IRequestPostProcessor<CreateOrderCommand, Result<Guid>>
{
    public Task Process(CreateOrderCommand request, Result<Guid> response, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[post] order created successfully? {Success}", response.IsSuccess);
        return Task.CompletedTask;
    }
}
