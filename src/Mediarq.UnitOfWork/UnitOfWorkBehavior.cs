using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Results;

namespace Mediarq.UnitOfWork;

/// <summary>
/// Pipeline behavior that commits a <see cref="IUnitOfWork"/> after a successful
/// <see cref="ITransactionalRequest"/>. Other requests pass through untouched, and a failed
/// <see cref="Result"/> response is not committed.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work committed after a successful transactional request.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="unitOfWork"/> is <see langword="null"/>.</exception>
    public UnitOfWorkBehavior(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        if (context.Request is not ITransactionalRequest)
        {
            return await handle().ConfigureAwait(false);
        }

        var response = await handle().ConfigureAwait(false);

        // Do not persist when the operation reported a failed Result.
        if (response is Result { IsFailure: true })
        {
            return response;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return response;
    }
}
