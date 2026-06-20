using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Pipeline.Behaviors;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Exceptions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Processors;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Common.Time;
using Mediarq.Core.Common.User;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Mediarq.Extensions;

/// <summary>
/// Provides extension methods for registering Mediarq core services
/// and components within an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Mediarq framework components (mediator, handlers, pipeline behaviors,
    /// validators, and context providers) into the application's dependency injection container,
    /// discovering handlers via a runtime assembly scan.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to configure.</param>
    /// <param name="isHttp">
    /// When <see langword="true"/>, registers <see cref="HttpUserContext"/> (reads the user from the
    /// current HTTP context); otherwise a <see cref="DefaultUserContext"/> is used. When <see langword="true"/>,
    /// also register an <c>IHttpContextAccessor</c> (e.g. via <c>services.AddHttpContextAccessor()</c>).
    /// </param>
    /// <param name="assemblies">
    /// The assemblies to scan for handlers, pipeline behaviors and validators. When none are provided,
    /// the entry assembly is scanned. The Mediarq assembly is always scanned for the built-in behaviors.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.</returns>
    /// <remarks>
    /// For a reflection-free, trimming/AOT-friendly alternative, use <see cref="AddMediarqCore"/> together
    /// with the compile-time generated <c>AddMediarqHandlers()</c> extension (provided by the Mediarq source generator).
    /// </remarks>
    [RequiresUnreferencedCode("AddMediarq discovers handlers via a runtime assembly scan, which is not trimming/AOT safe. Use AddMediarqCore() + the generated AddMediarqHandlers() instead.")]
    public static IServiceCollection AddMediarq(
        this IServiceCollection services,
        bool isHttp = false,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        AddCoreServices(services, isHttp);

        // Scan the caller-provided assemblies; when none are supplied, fall back to the entry assembly so
        // a consumer's handlers in the startup project are discovered automatically. The Mediarq assembly
        // is intentionally NOT scanned: its built-in behaviors are registered conditionally below, so an
        // app that has no validators/processors/exception-handlers pays nothing for them per request.
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

        if (scanTargets.Count > 0)
        {
            services.Scan(scan => scan
                .FromAssemblies(scanTargets)
                .AddClasses(c => c.AssignableToAny(
                    typeof(IRequestHandler<,>),
                    typeof(ICommandHandler<,>),
                    typeof(IQueryHandler<,>),
                    typeof(INotificationHandler<>)
                ))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IPipelineBehavior<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IStreamPipelineBehavior<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IRequestExceptionHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IStreamRequestHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IRequestPreProcessor<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IRequestPostProcessor<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
        }

        // Register the built-in plumbing behaviors only when a matching companion was discovered, so an
        // idle pipeline resolves no behavior at all. Logging/performance are opt-in (see
        // AddMediarqRequestLogging / AddMediarqPerformanceTracking).
        if (HasOpenGenericService(services, typeof(IRequestExceptionHandler<,>)))
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
        }

        if (HasOpenGenericService(services, typeof(IValidator<>)))
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        if (HasOpenGenericService(services, typeof(IRequestPreProcessor<>)))
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        }

        if (HasOpenGenericService(services, typeof(IRequestPostProcessor<,>)))
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
        }

        return services;
    }

    /// <summary>
    /// Registers only the Mediarq core services and the built-in pipeline behaviors, without any
    /// assembly scan. Combine with the compile-time generated <c>AddMediarqHandlers()</c> extension
    /// to register your handlers, custom behaviors and validators with no runtime reflection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to configure.</param>
    /// <param name="isHttp">See <see cref="AddMediarq"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddMediarqCore(isHttp: true)
    ///         .AddMediarqHandlers(); // generated at compile time
    /// </code>
    /// </example>
    public static IServiceCollection AddMediarqCore(
        this IServiceCollection services,
        bool isHttp = false)
    {
        ArgumentNullException.ThrowIfNull(services);

        AddCoreServices(services, isHttp);

        // The built-in plumbing behaviors live in the Mediarq assembly; register them explicitly since
        // this entry point does not scan any assembly. They are conditional behaviors, so each is omitted
        // from the pipeline at run time for request types that have no validator/processor/exception
        // handler — keeping the hot path cost-free while remaining available when needed.
        // Order: exception (outermost) -> validation -> pre -> handler -> post.
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

        return services;
    }

    /// <summary>
    /// Opt-in: registers the <see cref="LoggingBehavior{TRequest, TResponse}"/> that logs request start
    /// and completion. Not registered by default — add it when you want request logging. The behavior is
    /// active only when information-level logging is enabled.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    public static IServiceCollection AddMediarqRequestLogging(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }

    /// <summary>
    /// Opt-in: registers the <see cref="PerformanceBehavior{TRequest, TResponse}"/> that warns when a
    /// request runs longer than the threshold. Not registered by default — add it when you want
    /// performance tracking. The behavior is active only when warning-level logging is enabled.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    public static IServiceCollection AddMediarqPerformanceTracking(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        return services;
    }

    // True when any registration's service type is a closed form of the given open generic (e.g. a
    // concrete IValidator<SomeCommand> for typeof(IValidator<>)). Used to register a built-in plumbing
    // behavior only when there is something for it to do.
    private static bool HasOpenGenericService(IServiceCollection services, Type openGeneric)
    {
        for (var i = 0; i < services.Count; i++)
        {
            var serviceType = services[i].ServiceType;
            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == openGeneric)
            {
                return true;
            }
        }

        return false;
    }

    private static void AddCoreServices(IServiceCollection services, bool isHttp)
    {
        services.AddScoped<IHandlerResolver>(sp => new HandlerResolver(sp.GetService));
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<ISender>(sp => sp.GetRequiredService<IMediator>());
        services.AddScoped<IPublisher>(sp => sp.GetRequiredService<IMediator>());

        services.TryAddSingleton<IClock, SystemClock>();
        services.TryAddScoped<IRequestContextFactory, RequestContextFactory>();
        services.TryAddScoped<IPipelineExecutor, PipelineExecutor>();
        services.TryAddScoped<IStreamPipelineExecutor, StreamPipelineExecutor>();
        services.TryAddSingleton<INotificationPublisher, ParallelNotificationPublisher>();

        if (isHttp)
        {
            services.TryAddScoped<IUserContext, HttpUserContext>();
        }

        services.TryAddScoped<IUserContext, DefaultUserContext>();
    }
}
