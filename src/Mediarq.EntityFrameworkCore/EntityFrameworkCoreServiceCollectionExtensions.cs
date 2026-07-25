using Mediarq.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

    /// <summary>
    /// Registers <see cref="DomainEventsInterceptor"/> as a scoped <see cref="IInterceptor"/> on the
    /// application service provider, so any <see cref="DbContext"/> configured via <c>AddDbContext</c>
    /// (which wires that same provider for interceptor discovery) picks it up automatically — no need to
    /// add it explicitly in your own <c>AddDbContext(...)</c> call.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Scoped, not singleton: <see cref="DomainEventsInterceptor"/> depends on the scoped <c>IPublisher</c>,
    /// and EF Core resolves application-service-provider interceptors per <see cref="DbContext"/>
    /// construction, so a scoped registration correctly gets a fresh instance (and a fresh <c>IPublisher</c>)
    /// per scope instead of capturing the first one forever.
    /// </remarks>
    public static IServiceCollection AddMediarqDomainEvents(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IInterceptor, DomainEventsInterceptor>();
        return services;
    }
}
