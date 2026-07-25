using Mediarq.Core.Common.Requests.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.RabbitMQ;

/// <summary>
/// Extension methods that register the RabbitMQ publish forwarder and the consumer background service.
/// </summary>
/// <remarks>
/// Both require an <see cref="global::RabbitMQ.Client.IConnection"/> already registered in DI — this
/// package never owns the connection's lifecycle, so bring your own (e.g. a singleton built from
/// <c>ConnectionFactory.CreateConnectionAsync()</c>).
/// </remarks>
public static class RabbitMqServiceCollectionExtensions
{
    /// <summary>
    /// Forwards a specific notification type to its RabbitMQ exchange/routing key when it is published
    /// through Mediarq.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to forward.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqRabbitMqPublisher<TNotification>(this IServiceCollection services)
        where TNotification : class, IRabbitMqEvent
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<INotificationHandler<TNotification>, RabbitMqNotificationForwarder<TNotification>>();
        return services;
    }

    /// <summary>
    /// Registers a background service that declares <typeparamref name="TNotification"/>'s exchange,
    /// queue and binding, consumes it, and republishes each delivery through the real Mediarq pipeline
    /// via <see cref="Mediarq.Core.Mediators.IPublisher"/>.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to consume.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqRabbitMqSubscriber<TNotification>(this IServiceCollection services)
        where TNotification : class, IRabbitMqEvent
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<RabbitMqSubscriberHostedService<TNotification>>();
        return services;
    }
}
