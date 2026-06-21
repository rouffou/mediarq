using FluentValidation;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Outbox;
using Mediarq.Samples.WebApi.Domain;
using Mediarq.UnitOfWork;

namespace Mediarq.Samples.WebApi.Features.Orders;

public sealed record OrderLineInput(string Product, int Quantity, decimal UnitPrice);

/// <summary>
/// Creates an order. Implements <see cref="ITransactionalRequest"/> so the UnitOfWorkBehavior commits
/// the EF Core change tracker after the handler — atomically persisting the order AND the outbox event.
/// </summary>
public sealed record CreateOrderCommand(string Customer, List<OrderLineInput> Items)
    : ICommand<Result<Guid>>, ITransactionalRequest;

public sealed class CreateOrderHandler(AppDbContext db, IOutbox outbox)
    : ICommandHandler<CreateOrderCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Customer = request.Customer,
            Items = request.Items
                .Select(i => new OrderItem { Id = Guid.NewGuid(), Product = i.Product, Quantity = i.Quantity, UnitPrice = i.UnitPrice })
                .ToList(),
        };
        order.Total = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        db.Orders.Add(order);

        // Stage the integration event in the SAME unit of work. It is committed together with the order
        // (transactional outbox) and published afterwards by the OutboxProcessor — never lost, never
        // published without the order.
        outbox.Enqueue(new OrderPlacedEvent(order.Id, order.Customer, order.Total));

        return Task.FromResult(Result.Success(order.Id));
    }
}

/// <summary>FluentValidation validator, bridged into the Mediarq pipeline by AddMediarqFluentValidation().</summary>
public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Customer).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Items).NotEmpty().WithMessage("An order needs at least one line.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Product).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}
