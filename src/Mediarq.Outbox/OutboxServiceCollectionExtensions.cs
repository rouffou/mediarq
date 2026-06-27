using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediarq.Outbox;

/// <summary>
/// Extension methods that wire the Mediarq transactional outbox over an EF Core context.
/// </summary>
public static class OutboxServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="EfCoreOutbox{TContext}"/> as the scoped <see cref="IOutbox"/> and the
    /// <see cref="OutboxProcessor{TContext}"/> background service that publishes pending messages.
    /// </summary>
    /// <typeparam name="TContext">The EF Core <see cref="DbContext"/> that maps <see cref="OutboxMessage"/> (see <see cref="OutboxModelBuilderExtensions.ApplyMediarqOutbox"/>).</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">Optional callback to tune the processor (polling interval, batch size).</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Call after <c>AddMediarq</c>/<c>AddMediarqCore</c> and after registering <typeparamref name="TContext"/>
    /// (e.g. <c>AddDbContext</c>). Map the outbox table by calling <c>ApplyMediarqOutbox()</c> in your
    /// context's <c>OnModelCreating</c>, and enqueue events from your handlers via <see cref="IOutbox"/>.
    /// </remarks>
    public static IServiceCollection AddMediarqOutbox<TContext>(this IServiceCollection services, Action<OutboxOptions>? configure = null)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new OutboxOptions();
        configure?.Invoke(options);

        services.TryAddSingleton(options);
        services.AddScoped<IOutbox, EfCoreOutbox<TContext>>();
        services.AddHostedService<OutboxProcessor<TContext>>();

        return services;
    }
}
