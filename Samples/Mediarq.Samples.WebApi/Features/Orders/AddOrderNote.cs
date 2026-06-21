using System.ComponentModel.DataAnnotations;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Samples.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Samples.WebApi.Features.Orders;

/// <summary>
/// Attaches a note to an order. Validated with <c>System.ComponentModel.DataAnnotations</c> attributes
/// (the alternative to FluentValidation), bridged into the pipeline by AddMediarqDataAnnotations().
/// </summary>
public sealed record AddOrderNoteCommand : ICommand<Result>
{
    [Required]
    public Guid OrderId { get; init; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(200, MinimumLength = 1)]
    public string Note { get; init; } = string.Empty;
}

public sealed class AddOrderNoteHandler(AppDbContext db)
    : ICommandHandler<AddOrderNoteCommand, Result>
{
    public async Task<Result> Handle(AddOrderNoteCommand request, CancellationToken cancellationToken = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure(ResultError.NotFound("Order.NotFound", $"Order {request.OrderId} was not found."));

        order.Note = request.Note;
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
