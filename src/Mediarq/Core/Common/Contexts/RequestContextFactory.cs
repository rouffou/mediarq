using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Common.Time;
using Mediarq.Core.Common.User;

namespace Mediarq.Core.Common.Contexts;

public class RequestContextFactory : IRequestContextFactory
{
    private readonly IUserContext _userContext;
    private readonly IClock _clock;

    public RequestContextFactory(IUserContext userContext, IClock clock)
    {
        ArgumentNullException.ThrowIfNull(userContext);
        ArgumentNullException.ThrowIfNull(clock);
        _userContext = userContext;
        _clock = clock;
    }

    public object Create<TRequest, TResponse>(TRequest request,  CancellationToken cancellationToken = default) where TRequest : ICommandOrQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);

        var concreteRequestType = request.GetType();   // ex: CreateUserCommand
        var responseType = request.GetResponseType();  // ex: Result<Guid>

        var requestContextObj = CreateConcrete(concreteRequestType, responseType, request, cancellationToken);


        return requestContextObj ?? throw new InvalidOperationException($"Could not create RequestContext for request type {typeof(TRequest)} and response type {typeof(TResponse)}.");
    }

    private object CreateConcrete(Type requestType, Type responseType, object request, CancellationToken cancellationToken)
    {
        var contextType = typeof(RequestContext<,>).MakeGenericType(requestType, responseType);
        return Activator.CreateInstance(contextType, request, _userContext.UserId, cancellationToken);
    }
}
