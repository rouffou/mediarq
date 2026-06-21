using Mediarq.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.EntityFrameworkCore;

/// <summary>
/// Extension methods that wire an EF Core <see cref="DbContext"/> as the Mediarq unit of work.
/// </summary>
public static class EntityFrameworkCoreServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="EfCoreUnitOfWork{TContext}"/> as the scoped <see cref="IUnitOfWork"/> and the
    /// Mediarq unit-of-work behavior, so transactional commands commit through <typeparamref name="TContext"/>.
    /// </summary>
    /// <typeparam name="TContext">The EF Core <see cref="DbContext"/> type (registered separately, e.g. <c>AddDbContext</c>).</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqEntityFrameworkCore<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IUnitOfWork, EfCoreUnitOfWork<TContext>>();
        return services.AddMediarqUnitOfWork();
    }
}
