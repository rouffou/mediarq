using System.Text.Json;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mediarq.RabbitMQ;

/// <summary>
/// Declares <typeparamref name="TNotification"/>'s exchange/queue/binding and consumes it, deserializing
/// each delivery and republishing it through the real Mediarq pipeline via <see cref="IPublisher"/>, in
/// its own DI scope.
/// </summary>
/// <typeparam name="TNotification">The notification type to consume.</typeparam>
/// <remarks>
/// Acknowledges a delivery only after <see cref="IPublisher.Publish{TNotification}"/> completes
/// successfully; a deserialization failure or handler exception is logged and the delivery is
/// negatively acknowledged without requeue (so a poison message doesn't loop forever — route it to a
/// dead-letter exchange at the broker/queue-argument level if you need one). This package does not
/// implement duplicate-delivery detection: RabbitMQ's at-least-once delivery can redeliver a message a
/// consumer already processed (e.g. after a connection drop before the ack lands); if your handlers
/// aren't naturally idempotent, dedupe on a message id yourself.
/// </remarks>
internal sealed class RabbitMqSubscriberHostedService<TNotification> : BackgroundService
    where TNotification : class, IRabbitMqEvent
{
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMqSubscriberHostedService<TNotification>> _logger;
    private IChannel? _channel;

    public RabbitMqSubscriberHostedService(
        IConnection connection,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqSubscriberHostedService<TNotification>> logger)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(scopeFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken).ConfigureAwait(false);

        await _channel.ExchangeDeclareAsync(TNotification.Exchange, ExchangeType.Direct, durable: true, autoDelete: false, cancellationToken: stoppingToken)
            .ConfigureAwait(false);
        await _channel.QueueDeclareAsync(TNotification.Queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken)
            .ConfigureAwait(false);
        await _channel.QueueBindAsync(TNotification.Queue, TNotification.Exchange, TNotification.RoutingKey, cancellationToken: stoppingToken)
            .ConfigureAwait(false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnReceivedAsync;

        await _channel.BasicConsumeAsync(
            TNotification.Queue,
            autoAck: false,
            consumerTag: string.Empty,
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer,
            cancellationToken: stoppingToken).ConfigureAwait(false);

        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken).ConfigureAwait(false);
    }

    internal async Task OnReceivedAsync(object sender, BasicDeliverEventArgs args)
    {
        try
        {
            var notification = JsonSerializer.Deserialize<TNotification>(args.Body.Span);
            if (notification is not null)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<IPublisher>().Publish(notification, CancellationToken.None).ConfigureAwait(false);
            }

            await _channel!.BasicAckAsync(args.DeliveryTag, multiple: false).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process a RabbitMQ delivery for {NotificationType}.", typeof(TNotification).Name);
            await _channel!.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync().ConfigureAwait(false);
        }

        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }
}
