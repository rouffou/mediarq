/// <summary>
/// Represents a factory delegate used by the mediator and pipeline components
/// to resolve service instances (such as handlers, behaviors, or context providers)
/// from the application's dependency injection container.
/// </summary>
/// <param name="serviceType">
/// The <see cref="Type"/> of the service to resolve.
/// Typically, this will be an interface such as
/// <see cref="IRequestHandler{TRequest, TResponse}"/> or <see cref="IPipelineBehavior{TRequest, TResponse}"/>.
/// </param>
/// <returns>
/// An instance of the requested <paramref name="serviceType"/> if it is registered;
/// otherwise, <see langword="null"/>.
/// </returns>
/// <remarks>
/// The <see cref="ServiceFactory"/> delegate acts as an abstraction layer
/// between the mediator infrastructure and the underlying dependency injection framework.
/// 
/// This approach enables full inversion of control while allowing the mediator
/// to remain agnostic of any specific IoC container implementation
/// (e.g., Microsoft.Extensions.DependencyInjection, Autofac, SimpleInjector, etc.).
/// 
/// A typical implementation simply delegates resolution to the application's
/// <see cref="IServiceProvider"/>, for example:
/// <code>
/// ServiceFactory serviceFactory = type => serviceProvider.GetService(type);
/// </code>
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// var serviceFactory = new ServiceFactory(type => container.Resolve(type));
/// var handler = (IRequestHandler&lt;CreateUserCommand, Result&gt;)serviceFactory(typeof(IRequestHandler&lt;CreateUserCommand, Result&gt;));
/// </code>
/// </example>
public delegate object ServiceFactory(Type serviceType);
