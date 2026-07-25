using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Mediarq.Testing;

/// <summary>
/// Registration extension for <see cref="SpyMediator"/>.
/// </summary>
public static class SpyMediatorServiceCollectionExtensions
{
    /// <summary>
    /// Decorates the already-registered <see cref="IMediator"/> with <see cref="SpyMediator"/>, so every
    /// <c>Send</c>/<c>Publish</c>/<c>CreateStream</c> call — whether made through <see cref="IMediator"/>,
    /// <see cref="ISender"/> or <see cref="IPublisher"/> — is recorded while still running through the
    /// real pipeline.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Call after <c>AddMediarq</c>/<c>AddMediarqCore</c>. <see cref="ISender"/> and <see cref="IPublisher"/>
    /// are not decorated directly — both are already registered to resolve the current <see cref="IMediator"/>
    /// from the container, so decorating <see cref="IMediator"/> alone covers all three.
    /// </remarks>
    public static IServiceCollection AddMediarqSpy(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Decorate<IMediator, SpyMediator>();
        return services;
    }
}
