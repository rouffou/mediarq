using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Resolvers;

public static class TypeResponseExtensions
{
    public static Type GetResponseType(this object request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();

        var iFace = requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType &&
                                 (i.GetGenericTypeDefinition() == typeof(ICommandOrQuery<>) ||
                                  i.GetGenericTypeDefinition() == typeof(IRequest<>)));

        if (iFace is null)
        {
            throw new ArgumentNullException($"Request {requestType.Name} does not implement IRequest<>");
        }

        return iFace.GetGenericArguments()[0];
    }
}
