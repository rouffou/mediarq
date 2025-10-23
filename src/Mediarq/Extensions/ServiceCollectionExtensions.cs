using Mediarq.Core.Common.Pipeline.Behaviors;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Time;
using Mediarq.Core.Common.User;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediarq.Extensions;

/// <summary>
/// Provides extension methods for registering Mediarq core services
/// and components within an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Mediarq framework components (mediator, handlers, pipeline behaviors,
    /// validators, and context providers) into the application's dependency injection container.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> instance to configure.
    /// </param>
    /// <param name="isHttp">
    /// Indicates whether the application is HTTP-based. If <see langword="true"/>,
    /// the <see cref="HttpUserContext"/> is used to retrieve user information
    /// from the current HTTP request context; otherwise,
    /// a <see cref="DefaultUserContext"/> is registered instead.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    /// <remarks>
    /// This method sets up the Mediarq infrastructure by:
    /// <list type="bullet">
    /// <item><description>Registering the <see cref="IMediator"/> implementation (<see cref="Mediator"/>)</description></item>
    /// <item><description>Configuring <see cref="ServiceFactory"/> to resolve dependencies dynamically</description></item>
    /// <item><description>Adding support services such as <see cref="IClock"/> and <see cref="IRequestContextFactory"/></description></item>
    /// <item><description>Automatically discovering and registering request/command/query handlers</description></item>
    /// <item><description>Registering <see cref="IPipelineBehavior{TRequest,TResponse}"/> implementations</description></item>
    /// <item><description>Registering all <see cref="IValidator{T}"/> implementations</description></item>
    /// </list>
    ///
    /// Example usage in an ASP.NET Core application:
    /// <code>
    /// public void ConfigureServices(IServiceCollection services)
    /// {
    ///     services.AddMediarq(isHttp: true);
    /// }
    /// </code>
    ///
    /// The method performs an assembly scan on all loaded application dependencies
    /// whose names start with "Mediarq" to automatically discover and register
    /// framework components.
    /// </remarks>
    public static IServiceCollection AddMediarq(
        this IServiceCollection services,
        bool isHttp = false)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ServiceFactory>(sp => sp.GetService);
        services.AddScoped<IMediator, Mediator>();

        services.TryAddSingleton<IClock, SystemClock>();
        services.TryAddScoped<IRequestContextFactory, RequestContextFactory>();
        services.TryAddScoped<IPipelineExecutor, PipelineExecutor>();

        if (isHttp)
        {
            services.TryAddScoped<IUserContext, HttpUserContext>();
        }

        services.TryAddScoped<IUserContext, DefaultUserContext>();

        services.Scan(scan => scan
            .FromApplicationDependencies(a => a.GetName().Name!.StartsWith("Mediarq"))
            .AddClasses(c => c.AssignableToAny(
                typeof(IRequestHandler<,>),
                typeof(ICommandHandler<,>),
                typeof(IQueryHandler<,>)
            ))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(IPipelineBehavior<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
