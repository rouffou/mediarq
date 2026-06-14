using Mediarq.Core.Common.Requests.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediarq.FluentValidation;

/// <summary>
/// Dependency-injection helpers to bridge FluentValidation validators into Mediarq's validation pipeline.
/// </summary>
public static class FluentValidationServiceCollectionExtensions
{
    /// <summary>
    /// Registers the FluentValidation → Mediarq adapter as an open-generic <see cref="IValidator{T}"/>.
    /// Register your FluentValidation validators separately (e.g. <c>services.AddValidatorsFromAssembly(...)</c>).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqFluentValidation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(IValidator<>), typeof(FluentValidationValidator<>)));

        return services;
    }
}
