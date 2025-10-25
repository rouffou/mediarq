using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Common.User;

namespace Mediarq.Core.Common.Contexts;

/// <summary>
/// Represents a factory for creating request contexts.
/// </summary>
public class RequestContextFactory : IRequestContextFactory {
    private readonly IUserContext _userContext;
    /// <summary>
    /// The constructor for RequestContextFactory.
    /// </summary>
    /// <param name="userContext">The user context to get information about the user.</param>
    public RequestContextFactory(IUserContext userContext) {
        ArgumentNullException.ThrowIfNull(userContext);
        _userContext = userContext;
    }

    /// <see cref="IRequestContextFactory.Create{TRequest, TResponse}(TRequest, CancellationToken)"/>
    public object Create<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : ICommandOrQuery<TResponse> {
        ArgumentNullException.ThrowIfNull(request);

        Type concreteRequestType = request.GetType();
        Type responseType = request.GetResponseType();

        object requestContextObj = CreateConcrete(concreteRequestType, responseType, request, cancellationToken);


        return requestContextObj ?? throw new InvalidOperationException($"Could not create RequestContext for request type {typeof(TRequest)} and response type {typeof(TResponse)}.");
    }

    private object CreateConcrete(Type requestType, Type responseType, object request, CancellationToken cancellationToken) {
        Type contextType = typeof(RequestContext<,>).MakeGenericType(requestType, responseType);
        return Activator.CreateInstance(contextType, request, _userContext.UserId, cancellationToken);
    }
}
