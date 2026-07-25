using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Deferred;

/// <summary>Extension methods that register in-process deferred dispatch.</summary>
public static class DeferredServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IDeferredDispatcher"/> and the <see cref="DeferredDispatchHostedService"/>
    /// background worker that drains it, running each queued command/notification through the real
    /// <see cref="Mediarq.Core.Mediators.ISender"/>/<see cref="Mediarq.Core.Mediators.IPublisher"/>
    /// pipeline in its own DI scope.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">Optionally configures the queue's capacity and full-mode behavior.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqDeferredDispatch(
        this IServiceCollection services,
        Action<DeferredDispatchOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new DeferredDispatchOptions();
        configure?.Invoke(options);

        services.AddSingleton(_ => CreateChannel(options));
        services.AddSingleton<IDeferredDispatcher, ChannelDeferredDispatcher>();
        services.AddHostedService<DeferredDispatchHostedService>();

        return services;
    }

    private static Channel<Func<IServiceProvider, CancellationToken, Task>> CreateChannel(DeferredDispatchOptions options)
    {
        if (options.Capacity is { } capacity)
        {
            return Channel.CreateBounded<Func<IServiceProvider, CancellationToken, Task>>(new BoundedChannelOptions(capacity)
            {
                FullMode = options.FullMode,
                SingleReader = true,
            });
        }

        return Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>(new UnboundedChannelOptions
        {
            SingleReader = true,
        });
    }
}
