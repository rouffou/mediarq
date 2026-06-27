using Mediarq.Core.Common.Requests.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.DataAnnotations;

/// <summary>
/// Extension methods that register the DataAnnotations validator.
/// </summary>
public static class DataAnnotationsServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="DataAnnotationsValidator{T}"/> as an open-generic <see cref="IValidator{T}"/>,
    /// so every request is validated against its DataAnnotations attributes by the validation pipeline.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqDataAnnotations(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped(typeof(IValidator<>), typeof(DataAnnotationsValidator<>));
        return services;
    }
}
