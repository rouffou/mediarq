using Azure.Messaging.ServiceBus;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mediarq.AzureServiceBus;

/// <summary>
/// Processes <typeparamref name="TNotification"/>'s subscription, deserializing each message and
/// republishing it through the real Mediarq pipeline via <see cref="IPublisher"/>, in its own DI scope.
/// </summary>
/// <typeparam name="TNotification">The notification type to consume.</typeparam>
/// <remarks>
/// Completes a message only after <see cref="IPublisher.Publish{TNotification}"/> succeeds; a
/// deserialization failure or handler exception is logged and the message is moved straight to the
/// dead-letter subqueue (the Service Bus analogue of RabbitMQ's "nack without requeue") so a poison
/// message doesn't loop forever. This package does not implement duplicate-delivery detection.
/// </remarks>
internal sealed class AzureServiceBusSubscriberHostedService<TNotification> : BackgroundService
    where TNotification : class, IAzureServiceBusEvent
{
    private readonly ServiceBusClient _client;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AzureServiceBusSubscriberHostedService<TNotification>> _logger;
    private ServiceBusProcessor? _processor;

    public AzureServiceBusSubscriberHostedService(
        ServiceBusClient client,
        IServiceScopeFactory scopeFactory,
        ILogger<AzureServiceBusSubscriberHostedService<TNotification>> logger)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(scopeFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _client = client;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = _client.CreateProcessor(TNotification.TopicName, TNotification.SubscriptionName);
        _processor.ProcessMessageAsync += OnProcessMessageAsync;
        _processor.ProcessErrorAsync += OnProcessErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken).ConfigureAwait(false);
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken).ConfigureAwait(false);
    }

    internal async Task OnProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var notification = args.Message.Body.ToObjectFromJson<TNotification>();
            if (notification is not null)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<IPublisher>().Publish(notification, args.CancellationToken).ConfigureAwait(false);
            }

            await args.CompleteMessageAsync(args.Message, args.CancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process an Azure Service Bus message for {NotificationType}.", typeof(TNotification).Name);
            await args.DeadLetterMessageAsync(args.Message, cancellationToken: args.CancellationToken).ConfigureAwait(false);
        }
    }

    internal Task OnProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Azure Service Bus processor error for {NotificationType}.", typeof(TNotification).Name);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null)
        {
            await _processor.StopProcessingAsync(cancellationToken).ConfigureAwait(false);
            await _processor.DisposeAsync().ConfigureAwait(false);
        }

        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }
}
