using Mediarq.Core.Common.Requests.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.AzureServiceBus;

/// <summary>
/// Extension methods that register the Azure Service Bus publish forwarder and the consumer background
/// service.
/// </summary>
/// <remarks>
/// Both require a <see cref="global::Azure.Messaging.ServiceBus.ServiceBusClient"/> already registered in
/// DI — this package never owns the client's lifecycle, so bring your own (e.g. a singleton built from
/// a connection string or <c>DefaultAzureCredential</c>).
/// </remarks>
public static class AzureServiceBusServiceCollectionExtensions
{
    /// <summary>
    /// Forwards a specific notification type to its Azure Service Bus topic when it is published through
    /// Mediarq.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to forward.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqAzureServiceBusPublisher<TNotification>(this IServiceCollection services)
        where TNotification : class, IAzureServiceBusEvent
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<INotificationHandler<TNotification>, AzureServiceBusNotificationForwarder<TNotification>>();
        return services;
    }

    /// <summary>
    /// Registers a background service that processes <typeparamref name="TNotification"/>'s
    /// subscription and republishes each message through the real Mediarq pipeline via
    /// <see cref="Mediarq.Core.Mediators.IPublisher"/>.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to consume.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqAzureServiceBusSubscriber<TNotification>(this IServiceCollection services)
        where TNotification : class, IAzureServiceBusEvent
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<AzureServiceBusSubscriberHostedService<TNotification>>();
        return services;
    }
}
