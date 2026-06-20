using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Polly;

/// <summary>
/// Extension methods that register the Mediarq resilience behavior.
/// </summary>
public static class ResilienceServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="ResilienceBehavior{TRequest, TResponse}"/> so <see cref="IResilientRequest"/>
    /// requests run through their named Polly resilience pipeline.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Configure the pipelines separately, e.g. <c>services.AddResiliencePipeline("name", b =&gt; b.AddRetry(...))</c>.
    /// Call after <c>AddMediarq</c>/<c>AddMediarqCore</c>.
    /// </remarks>
    public static IServiceCollection AddMediarqResilience(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ResilienceBehavior<,>));
        return services;
    }
}
