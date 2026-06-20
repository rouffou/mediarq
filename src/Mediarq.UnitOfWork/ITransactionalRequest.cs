namespace Mediarq.UnitOfWork;

/// <summary>
/// Marks a request that should be wrapped in a unit of work: after the handler succeeds, the
/// <see cref="UnitOfWorkBehavior{TRequest, TResponse}"/> commits via <see cref="IUnitOfWork"/>.
/// </summary>
/// <remarks>Typically applied to commands. A failed <c>Result</c> response is not committed.</remarks>
public interface ITransactionalRequest;
