using Mediarq.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.EntityFrameworkCore;

/// <summary>
/// An <see cref="IUnitOfWork"/> backed by an EF Core <typeparamref name="TContext"/>: committing
/// persists pending changes via <see cref="DbContext.SaveChangesAsync(CancellationToken)"/>.
/// </summary>
/// <typeparam name="TContext">The EF Core <see cref="DbContext"/> type.</typeparam>
public sealed class EfCoreUnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext _context;

    /// <summary>Initializes a new instance wrapping <paramref name="context"/>.</summary>
    /// <param name="context">The EF Core context to commit.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/>.</exception>
    public EfCoreUnitOfWork(TContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
