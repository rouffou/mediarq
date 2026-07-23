using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Streaming;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.MediatRCompat;

/// <summary>
/// Registers the MediatR compatibility shim: an <c>AddMediarqMediatRCompat()</c> that discovers your
/// existing <c>MediatR.IRequestHandler</c>/<c>INotificationHandler</c>/<c>IStreamRequestHandler</c>
/// implementations and wires them through Mediarq's real dispatch pipeline via
/// <see cref="MediatRCompatMediator"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scans the given assemblies for MediatR-shaped request, notification and stream request handlers,
    /// and registers each of them behind a <see cref="MediatRCompatMediator"/> that dispatches through
    /// Mediarq's real <c>ISender</c>/<c>IPublisher</c>. Call this in addition to (after) <c>AddMediarq</c>
    /// or <c>AddMediarqCore</c>, which must already have registered Mediarq's own dispatch services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to configure.</param>
    /// <param name="assemblies">
    /// The assemblies to scan for MediatR-shaped handlers. When none are provided, the entry assembly
    /// is scanned.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddMediarq(isHttp: false, typeof(Program).Assembly)
    ///         .AddMediarqMediatRCompat(typeof(Program).Assembly);
    /// </code>
    /// </example>
    [RequiresUnreferencedCode("AddMediarqMediatRCompat discovers MediatR-shaped handlers via a runtime assembly scan, which is not trimming/AOT safe.")]
    public static IServiceCollection AddMediarqMediatRCompat(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<MediatRCompatMediator>();
        services.AddScoped<MediatR.IMediator>(sp => sp.GetRequiredService<MediatRCompatMediator>());
        services.AddScoped<MediatR.ISender>(sp => sp.GetRequiredService<MediatRCompatMediator>());
        services.AddScoped<MediatR.IPublisher>(sp => sp.GetRequiredService<MediatRCompatMediator>());

        var scanTargets = new HashSet<Assembly>();
        if (assemblies is { Length: > 0 })
        {
            foreach (var assembly in assemblies)
            {
                ArgumentNullException.ThrowIfNull(assembly);
                scanTargets.Add(assembly);
            }
        }
        else
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly is not null)
            {
                scanTargets.Add(entryAssembly);
            }
        }

        foreach (var assembly in scanTargets)
        {
            RegisterCompatHandlers(services, assembly);
        }

        return services;
    }

    [RequiresUnreferencedCode("Scans the assembly's types via reflection.")]
    private static void RegisterCompatHandlers(IServiceCollection services, Assembly assembly)
    {
        foreach (var type in GetLoadableTypes(assembly))
        {
            if (!type.IsClass || type.IsAbstract)
            {
                continue;
            }

            foreach (var contract in type.GetInterfaces())
            {
                if (!contract.IsGenericType)
                {
                    continue;
                }

                var definition = contract.GetGenericTypeDefinition();
                var args = contract.GetGenericArguments();

                // The concrete handler is registered under its own type (not the MediatR interface): a
                // notification can have several handlers sharing the same MediatR.INotificationHandler<T>,
                // so the adapter factory below must capture *this* concrete instance, not "whichever
                // implementation of the interface DI resolves last".
                services.AddScoped(type);

                if (definition == typeof(MediatR.IRequestHandler<,>))
                {
                    var adapterType = typeof(CompatRequestHandlerAdapter<,>).MakeGenericType(args);
                    var wrapperType = typeof(CompatRequestWrapper<,>).MakeGenericType(args);
                    services.AddScoped(
                        typeof(IRequestHandler<,>).MakeGenericType(wrapperType, args[1]),
                        BuildAdapterFactory(type, adapterType));
                }
                else if (definition == typeof(MediatR.IRequestHandler<>))
                {
                    var adapterType = typeof(CompatCommandHandlerAdapter<>).MakeGenericType(args);
                    var wrapperType = typeof(CompatCommandWrapper<>).MakeGenericType(args);
                    var factory = BuildAdapterFactory(type, adapterType);
                    // Mediarq dispatches a void command as ICommandOrQuery<Unit> and resolves
                    // IRequestHandler<TRequest, Unit> directly - register the adapter under both the
                    // one-arg form (for symmetry/direct resolution) and the two-arg Unit form Mediarq's
                    // pipeline actually looks up.
                    services.AddScoped(typeof(IRequestHandler<>).MakeGenericType(wrapperType), factory);
                    services.AddScoped(typeof(IRequestHandler<,>).MakeGenericType(wrapperType, typeof(Unit)), factory);
                }
                else if (definition == typeof(MediatR.INotificationHandler<>))
                {
                    var adapterType = typeof(CompatNotificationHandlerAdapter<>).MakeGenericType(args);
                    var wrapperType = typeof(CompatNotificationWrapper<>).MakeGenericType(args);
                    services.AddScoped(
                        typeof(INotificationHandler<>).MakeGenericType(wrapperType),
                        BuildAdapterFactory(type, adapterType));
                }
                else if (definition == typeof(MediatR.IStreamRequestHandler<,>))
                {
                    var adapterType = typeof(CompatStreamRequestHandlerAdapter<,>).MakeGenericType(args);
                    var wrapperType = typeof(CompatStreamRequestWrapper<,>).MakeGenericType(args);
                    services.AddScoped(
                        typeof(IStreamRequestHandler<,>).MakeGenericType(wrapperType, args[1]),
                        BuildAdapterFactory(type, adapterType));
                }
            }
        }
    }

    // Builds a factory that resolves the concrete MediatR-shaped handler by its own type (see the
    // comment above the scan loop) and wraps it in the given adapter via its single-parameter constructor.
    [RequiresUnreferencedCode("Constructs the adapter type via reflection.")]
    private static Func<IServiceProvider, object> BuildAdapterFactory(Type concreteHandlerType, Type adapterType)
    {
        var constructor = adapterType.GetConstructors().Single();
        return sp => constructor.Invoke([sp.GetRequiredService(concreteHandlerType)]);
    }

    [RequiresUnreferencedCode("Scans the assembly's types via reflection.")]
    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
    }
}
