using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.User;

namespace Mediarq.Core.Common.Contexts;

/// <summary>
/// Default factory that creates <see cref="RequestContext{TRequest, TResponse}"/> instances,
/// stamping each request with the current user obtained from <see cref="IUserContext"/>.
/// </summary>
public class RequestContextFactory : IRequestContextFactory
{
    private readonly IUserContext _userContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestContextFactory"/> class.
    /// </summary>
    /// <param name="userContext">The user context used to identify who issued the request.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="userContext"/> is <see langword="null"/>.</exception>
    public RequestContextFactory(IUserContext userContext)
    {
        ArgumentNullException.ThrowIfNull(userContext);
        _userContext = userContext;
    }

    /// <inheritdoc />
    public RequestContext<TRequest, TResponse> Create<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : ICommandOrQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);

        return new RequestContext<TRequest, TResponse>(request, _userContext.UserId, cancellationToken);
    }
}
