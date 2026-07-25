using System.Collections.Concurrent;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Pipeline;

/// <summary>
/// Per-container memo of which closed <see cref="IPipelineBehavior{TRequest, TResponse}"/> types have
/// zero registrations, so a repeat dispatch can skip resolving an empty <c>IEnumerable&lt;&gt;</c> for
/// that type. Register as a singleton — never as a static/process-wide cache — since a process can host
/// multiple DI containers (e.g. across tests) with different registrations for the same closed types.
/// </summary>
/// <remarks>
/// Only ever records "definitely zero behaviors are registered": a structural, DI-registration-time fact
/// that cannot change once the container is built, so it is safe to cache forever after the first
/// observation. It deliberately never records "no <em>active</em> behavior this time" —
/// <see cref="IConditionalPipelineBehavior.IsActive"/> is per-request runtime state and must be
/// re-evaluated on every dispatch.
/// </remarks>
public sealed class PipelineBehaviorRegistrationCache
{
    private readonly ConcurrentDictionary<Type, bool> _knownEmpty = new();

    internal bool IsKnownEmpty<TRequest, TResponse>() where TRequest : ICommandOrQuery<TResponse>
        => _knownEmpty.ContainsKey(typeof(IPipelineBehavior<TRequest, TResponse>));

    internal void MarkKnownEmpty<TRequest, TResponse>() where TRequest : ICommandOrQuery<TResponse>
        => _knownEmpty.TryAdd(typeof(IPipelineBehavior<TRequest, TResponse>), true);
}
