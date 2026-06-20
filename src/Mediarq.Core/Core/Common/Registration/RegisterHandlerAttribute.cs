using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Core.Common.Registration;

/// <summary>
/// Overrides the DI lifetime used to register a Mediarq handler, behavior or validator.
/// </summary>
/// <remarks>
/// Honored by the compile-time generated <c>AddMediarqHandlers()</c>. The default lifetime is
/// <see cref="ServiceLifetime.Scoped"/>. Use with care: a longer-lived component must not capture a
/// shorter-lived dependency.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RegisterHandlerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterHandlerAttribute"/> class.
    /// </summary>
    /// <param name="lifetime">The DI lifetime to use when registering the decorated type.</param>
    public RegisterHandlerAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        Lifetime = lifetime;
    }

    /// <summary>Gets the DI lifetime to use when registering the decorated type.</summary>
    public ServiceLifetime Lifetime { get; }
}
