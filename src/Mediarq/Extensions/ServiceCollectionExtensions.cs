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
using System.Reflection;

namespace Mediarq.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediarq(
        this IServiceCollection services,
        bool isHttp,
        params Assembly[] assemblies)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddScoped<ServiceFactory>(sp => sp.GetService!);
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
