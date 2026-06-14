namespace Mediarq.UnitOfWork;

/// <summary>
/// Abstraction over a unit of work that persists the changes accumulated while handling a request.
/// Implement it over your data access (e.g. an EF Core <c>DbContext</c>).
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persists all pending changes.</summary>
    /// <param name="cancellationToken">A token to observe while saving.</param>
    /// <returns>The number of state entries written, when applicable.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
