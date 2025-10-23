namespace Mediarq.Core;

/// <summary>
/// Provides extension methods for creating <see cref="ServiceFactory"/> delegates
/// from existing dependency injection containers.
/// </summary>
public static class ServiceFactoryExtentions
{
    /// <summary>
    /// Creates a <see cref="ServiceFactory"/> delegate using the provided
    /// <see cref="IServiceProvider"/> for service resolution.
    /// </summary>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance used to resolve dependencies.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceFactory"/> delegate that retrieves service instances
    /// by delegating calls to <see cref="IServiceProvider.GetService(Type)"/>.
    /// </returns>
    /// <remarks>
    /// This method is typically used during mediator setup to connect the mediator
    /// infrastructure to the application's dependency injection container without
    /// introducing a compile-time dependency on any specific IoC framework.
    /// 
    /// Example integration with ASP.NET Core:
    /// <code>
    /// var serviceFactory = ServiceFactoryExtentions.FromServiceProvider(app.Services);
    /// var mediator = new Mediator(serviceFactory, requestContextFactory, pipelineExecutor);
    /// </code>
    /// </remarks>
    public static ServiceFactory FromServiceProvider(IServiceProvider serviceProvider)
        => serviceProvider.GetService;
}
