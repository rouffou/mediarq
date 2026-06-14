using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.UnitOfWork;

/// <summary>
/// Extension methods that register the Mediarq unit-of-work behavior.
/// </summary>
public static class UnitOfWorkServiceCollectionExtensions
{
    /// <summary>
    /// Registers <typeparamref name="TUnitOfWork"/> as the scoped <see cref="IUnitOfWork"/> and the
    /// <see cref="UnitOfWorkBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TUnitOfWork">The unit-of-work implementation.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqUnitOfWork<TUnitOfWork>(this IServiceCollection services)
        where TUnitOfWork : class, IUnitOfWork
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IUnitOfWork, TUnitOfWork>();
        return services.AddMediarqUnitOfWork();
    }

    /// <summary>
    /// Registers the <see cref="UnitOfWorkBehavior{TRequest, TResponse}"/>. Use this overload when an
    /// <see cref="IUnitOfWork"/> is already registered (for example your EF Core <c>DbContext</c>).
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqUnitOfWork(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
        return services;
    }
}
