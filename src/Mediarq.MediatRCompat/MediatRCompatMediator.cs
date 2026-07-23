using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Mediators;

namespace Mediarq.MediatRCompat;

/// <summary>
/// Implements MediatR's own <c>IMediator</c> (<c>ISender</c> + <c>IPublisher</c>) on top of Mediarq's
/// real dispatch pipeline. Register it in place of MediatR's <c>Mediator</c> (via
/// <see cref="ServiceCollectionExtensions.AddMediarqMediatRCompat"/>) to keep existing
/// <c>MediatR.IRequest</c>/<c>IRequestHandler</c>/<c>INotification</c>/<c>INotificationHandler</c>/
/// <c>IStreamRequest</c>/<c>IStreamRequestHandler</c> classes compiling and running unchanged while you
/// convert them to native Mediarq types (e.g. with <c>Mediarq.Analyzers</c>) at your own pace.
/// </summary>
/// <remarks>
/// Every request/notification's concrete type is wrapped into a Mediarq
/// <see cref="ICommandOrQuery{TResponse}"/> / <c>ICommand</c> / <c>INotification</c> /
/// <see cref="IStreamRequest{TResponse}"/> so it flows through the same pipeline behaviors
/// (validation, logging, ...) as native Mediarq requests. Building that wrapper for an arbitrary
/// runtime type requires reflection once per concrete type (cached thereafter) — this is an explicit
/// migration/compatibility path, not the Native-AOT hot path. Prefer converting a handler to a native
/// Mediarq type when you want reflection-free dispatch for it.
/// </remarks>
[SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters",
    Justification = "Mirrors MediatR.IMediator's own Send/Publish/CreateStream overload shape exactly, " +
        "by design -- existing MediatR call sites must keep compiling unchanged.")]
public sealed class MediatRCompatMediator : MediatR.IMediator
{
    private const string ReflectionJustification =
        "MediatRCompatMediator bridges MediatR-shaped requests, whose concrete type is only known at " +
        "runtime, into Mediarq's dispatch pipeline. This is an explicit migration/compatibility path, " +
        "not the Native AOT hot path -- see AddMediarqCore() + the generated AddMediarqHandlers() for that.";

    private static readonly ConcurrentDictionary<Type, Type> RequestWrapperTypes = new();
    private static readonly ConcurrentDictionary<Type, Type> StreamWrapperTypes = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> DynamicSendMethods = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> DynamicStreamMethods = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> DynamicPublishMethods = new();

    private static readonly MethodInfo InvokeSendWithResponseMethod =
        typeof(MediatRCompatMediator).GetMethod(nameof(InvokeSendWithResponseAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo InvokeSendVoidMethod =
        typeof(MediatRCompatMediator).GetMethod(nameof(InvokeSendVoidAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo InvokeCreateStreamMethod =
        typeof(MediatRCompatMediator).GetMethod(nameof(InvokeCreateStreamAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo PublishOpenGenericMethod =
        typeof(MediatRCompatMediator).GetMethods()
            .Single(m => m.Name == nameof(Publish) && m.IsGenericMethodDefinition);

    private readonly ISender _sender;
    private readonly IPublisher _publisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediatRCompatMediator"/> class.
    /// </summary>
    /// <param name="sender">Mediarq's real request sender, resolved from the container.</param>
    /// <param name="publisher">Mediarq's real notification publisher, resolved from the container.</param>
    public MediatRCompatMediator(ISender sender, IPublisher publisher)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(publisher);

        _sender = sender;
        _publisher = publisher;
    }

    /// <inheritdoc cref="MediatR.ISender.Send{TResponse}(MediatR.IRequest{TResponse}, CancellationToken)" />
    [RequiresUnreferencedCode(ReflectionJustification)]
    [RequiresDynamicCode(ReflectionJustification)]
    public Task<TResponse> Send<TResponse>(MediatR.IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var wrapperType = RequestWrapperTypes.GetOrAdd(
            requestType,
            static (rt, responseType) => typeof(CompatRequestWrapper<,>).MakeGenericType(rt, responseType),
            typeof(TResponse));
        var wrapped = (ICommandOrQuery<TResponse>)Activator.CreateInstance(wrapperType, request)!;
        return _sender.Send(wrapped, cancellationToken);
    }

    /// <inheritdoc cref="MediatR.ISender.Send{TRequest}(TRequest, CancellationToken)" />
    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : MediatR.IRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var wrapped = new CompatCommandWrapper<TRequest>(request);
        return _sender.Send(wrapped, cancellationToken);
    }

    /// <inheritdoc cref="MediatR.ISender.Send(object, CancellationToken)" />
    [RequiresUnreferencedCode(ReflectionJustification)]
    [RequiresDynamicCode(ReflectionJustification)]
    public async Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var method = DynamicSendMethods.GetOrAdd(requestType, BuildDynamicSendMethod);
        var task = (Task<object?>)method.Invoke(this, [request, cancellationToken])!;
        return await task.ConfigureAwait(false);
    }

    /// <inheritdoc cref="MediatR.ISender.CreateStream{TResponse}(MediatR.IStreamRequest{TResponse}, CancellationToken)" />
    [RequiresUnreferencedCode(ReflectionJustification)]
    [RequiresDynamicCode(ReflectionJustification)]
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(MediatR.IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var wrapperType = StreamWrapperTypes.GetOrAdd(
            requestType,
            static (rt, responseType) => typeof(CompatStreamRequestWrapper<,>).MakeGenericType(rt, responseType),
            typeof(TResponse));
        var wrapped = (IStreamRequest<TResponse>)Activator.CreateInstance(wrapperType, request)!;
        return _sender.CreateStream(wrapped, cancellationToken);
    }

    /// <inheritdoc cref="MediatR.ISender.CreateStream(object, CancellationToken)" />
    [RequiresUnreferencedCode(ReflectionJustification)]
    [RequiresDynamicCode(ReflectionJustification)]
    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var responseType = GetStreamResponseType(requestType)
            ?? throw new ArgumentException($"'{requestType}' does not implement MediatR.IStreamRequest<TResponse>.", nameof(request));
        var method = DynamicStreamMethods.GetOrAdd(requestType, _ => InvokeCreateStreamMethod.MakeGenericMethod(responseType));
        return (IAsyncEnumerable<object?>)method.Invoke(this, [request, cancellationToken])!;
    }

    /// <inheritdoc cref="MediatR.IPublisher.Publish{TNotification}(TNotification, CancellationToken)" />
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : MediatR.INotification
    {
        ArgumentNullException.ThrowIfNull(notification);

        var wrapped = new CompatNotificationWrapper<TNotification>(notification);
        return _publisher.Publish(wrapped, cancellationToken);
    }

    /// <inheritdoc cref="MediatR.IPublisher.Publish(object, CancellationToken)" />
    [RequiresUnreferencedCode(ReflectionJustification)]
    [RequiresDynamicCode(ReflectionJustification)]
    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (notification is not MediatR.INotification)
        {
            throw new ArgumentException($"'{notification.GetType()}' does not implement MediatR.INotification.", nameof(notification));
        }

        var notificationType = notification.GetType();
        var method = DynamicPublishMethods.GetOrAdd(notificationType, static nt => PublishOpenGenericMethod.MakeGenericMethod(nt));
        return (Task)method.Invoke(this, [notification, cancellationToken])!;
    }

    private async Task<object?> InvokeSendWithResponseAsync<TResponse>(object request, CancellationToken cancellationToken)
    {
        var response = await Send((MediatR.IRequest<TResponse>)request, cancellationToken).ConfigureAwait(false);
        return response;
    }

    private async Task<object?> InvokeSendVoidAsync<TRequest>(object request, CancellationToken cancellationToken)
        where TRequest : MediatR.IRequest
    {
        await Send((TRequest)request, cancellationToken).ConfigureAwait(false);
        return null;
    }

    private async IAsyncEnumerable<object?> InvokeCreateStreamAsync<TResponse>(object request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var item in CreateStream((MediatR.IStreamRequest<TResponse>)request, cancellationToken).ConfigureAwait(false))
        {
            yield return item;
        }
    }

    private static MethodInfo BuildDynamicSendMethod(Type requestType)
    {
        var responseType = GetRequestResponseType(requestType);
        if (responseType is not null)
        {
            return InvokeSendWithResponseMethod.MakeGenericMethod(responseType);
        }

        if (!typeof(MediatR.IRequest).IsAssignableFrom(requestType))
        {
            throw new ArgumentException($"'{requestType}' does not implement MediatR.IRequest or MediatR.IRequest<TResponse>.", nameof(requestType));
        }

        return InvokeSendVoidMethod.MakeGenericMethod(requestType);
    }

    private static Type? GetRequestResponseType(Type requestType) => GetClosedGenericArgument(requestType, typeof(MediatR.IRequest<>));

    private static Type? GetStreamResponseType(Type requestType) => GetClosedGenericArgument(requestType, typeof(MediatR.IStreamRequest<>));

    private static Type? GetClosedGenericArgument(Type concreteType, Type openGenericInterface)
    {
        foreach (var candidate in concreteType.GetInterfaces())
        {
            if (candidate.IsGenericType && candidate.GetGenericTypeDefinition() == openGenericInterface)
            {
                return candidate.GetGenericArguments()[0];
            }
        }

        return null;
    }
}
