using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediarq.Saga;

/// <summary>
/// Extension methods that register Mediarq saga / process-manager infrastructure.
/// </summary>
public static class SagaServiceCollectionExtensions
{
    /// <summary>
    /// Registers a process-lifetime <see cref="InMemorySagaStore{TState}"/> as
    /// <see cref="ISagaStore{TState}"/> for <typeparamref name="TState"/>, if one isn't already registered.
    /// </summary>
    /// <typeparam name="TState">The saga state type stored.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Register your own <see cref="ISagaStore{TState}"/> (e.g. backed by a database) before calling this
    /// method to use it instead — the in-memory default does not survive a process restart.
    /// </remarks>
    public static IServiceCollection AddMediarqSaga<TState>(this IServiceCollection services)
        where TState : class, ISagaState
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<ISagaStore<TState>, InMemorySagaStore<TState>>();
        return services;
    }
}
