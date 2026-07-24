using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediarq.Quartz;

/// <summary>
/// Extension methods that register Mediarq's Quartz job type.
/// </summary>
public static class QuartzServiceCollectionExtensions
{
    /// <summary>
    /// Registers the job type Quartz invokes for commands scheduled via <see cref="QuartzSchedulingExtensions"/>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Call after <c>AddMediarq</c>/<c>AddMediarqCore</c>. Also requires Quartz itself to be configured
    /// (<c>services.AddQuartz()</c>, from <c>Quartz.Extensions.DependencyInjection</c>) so the job
    /// resolves scoped services correctly.
    /// </remarks>
    public static IServiceCollection AddMediarqQuartz(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddTransient<MediarqQuartzJob>();
        return services;
    }
}
