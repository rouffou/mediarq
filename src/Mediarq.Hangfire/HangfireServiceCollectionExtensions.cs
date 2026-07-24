using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediarq.Hangfire;

/// <summary>
/// Extension methods that register Mediarq's Hangfire job dispatcher.
/// </summary>
public static class HangfireServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IMediarqJobDispatcher"/>, the job method Hangfire invokes for commands
    /// scheduled via <see cref="HangfireSchedulingExtensions"/>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Call after <c>AddMediarq</c>/<c>AddMediarqCore</c>. Also requires Hangfire itself to be configured
    /// (<c>AddHangfire(...)</c>, <c>AddHangfireServer()</c>) with an activator that resolves scoped
    /// services — see the package README.
    /// </remarks>
    public static IServiceCollection AddMediarqHangfire(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<IMediarqJobDispatcher, MediarqJobDispatcher>();
        return services;
    }
}
