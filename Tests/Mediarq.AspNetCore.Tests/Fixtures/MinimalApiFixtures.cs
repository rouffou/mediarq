using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace Mediarq.AspNetCore.Tests.Fixtures;

public sealed record OrderDto(Guid Id, string Customer);

[MediarqGet("/orders/{id}")]
public sealed record GetOrder(Guid Id) : IQuery<Result<OrderDto>>;

public sealed class GetOrderHandler : IQueryHandler<GetOrder, Result<OrderDto>>
{
    public Task<Result<OrderDto>> Handle(GetOrder request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Store.Orders.TryGetValue(request.Id, out var dto)
            ? Result.Success(dto)
            : Result.Failure<OrderDto>(ResultError.NotFound("Order.NotFound", "Order not found.")));
}

[MediarqPost("/orders")]
public sealed record CreateOrder(string Customer) : ICommand<Result<Guid>>;

public sealed class CreateOrderHandler : ICommandHandler<CreateOrder, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateOrder request, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        Store.Orders[id] = new OrderDto(id, request.Customer);
        return Task.FromResult(Result.Success(id));
    }
}

[MediarqDelete("/orders/{id}")]
public sealed record DeleteOrder(Guid Id) : ICommand;

public sealed class DeleteOrderHandler : ICommandHandler<DeleteOrder>
{
    public Task Handle(DeleteOrder request, CancellationToken cancellationToken = default)
    {
        Store.Orders.Remove(request.Id);
        return Task.CompletedTask;
    }
}

// Deliberately not attributed -- must not be mapped by MapMediarq.
public sealed record UnmappedCommand : ICommand;

public sealed class UnmappedCommandHandler : ICommandHandler<UnmappedCommand>
{
    public Task Handle(UnmappedCommand request, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal static class Store
{
    public static readonly Dictionary<Guid, OrderDto> Orders = new();
}
