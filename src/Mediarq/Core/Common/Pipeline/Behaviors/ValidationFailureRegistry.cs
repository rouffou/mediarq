using System.Collections.Concurrent;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

/// <summary>
/// Maps a response type to a delegate that turns a <see cref="ValidationError"/> into a failed value
/// of that type. The compile-time generated <c>AddMediarqHandlers()</c> populates it for every
/// <see cref="Result{T}"/> response, so <see cref="ValidationBehavior{TRequest, TResponse}"/> can
/// short-circuit with a failed result without reflection or dynamic code (trimming/AOT friendly).
/// </summary>
/// <remarks>
/// When the registry has no entry for a response type (for example with the reflection-based
/// <c>AddMediarq</c> assembly scan), <see cref="ValidationBehavior{TRequest, TResponse}"/> falls back
/// to building the factory dynamically. That fallback is not used on Native AOT.
/// </remarks>
public static class ValidationFailureRegistry
{
    private static readonly ConcurrentDictionary<Type, object> Factories = new();

    /// <summary>
    /// Registers the validation-failure factory for <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">The response type the factory produces (e.g. <see cref="Result{T}"/>).</typeparam>
    /// <param name="factory">Builds a failed <typeparamref name="TResponse"/> from a <see cref="ValidationError"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory"/> is <see langword="null"/>.</exception>
    public static void Register<TResponse>(Func<ValidationError, TResponse> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        Factories[typeof(TResponse)] = factory;
    }

    /// <summary>Gets the registered factory for <typeparamref name="TResponse"/>, or <see langword="null"/> if none.</summary>
    internal static Func<ValidationError, TResponse>? Get<TResponse>()
        => Factories.TryGetValue(typeof(TResponse), out var factory) ? (Func<ValidationError, TResponse>)factory : null;
}
