using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Resolvers;

/// <summary>
/// Provides extension methods related to request type analysis within the Mediarq pipeline.
/// </summary>
/// <remarks>
/// The <see cref="TypeResponseExtensions"/> class contains helper methods used to extract
/// metadata from request objects implementing Mediarq interfaces such as <see cref="ICommandOrQuery{TResponse}"/>
/// or <see cref="IRequest{TResponse}"/>.  
/// 
/// It is typically used internally by the Mediarq pipeline to dynamically determine the
/// expected response type for a given request instance, without requiring compile-time knowledge
/// of the specific generic type.
/// </remarks>
public static class TypeResponseExtensions
{
    /// <summary>
    /// Retrieves the response type (<c>TResponse</c>) of a given request instance implementing
    /// <see cref="ICommandOrQuery{TResponse}"/> or <see cref="IRequest{TResponse}"/>.
    /// </summary>
    /// <param name="request">The request object whose response type is to be determined.</param>
    /// <returns>
    /// A <see cref="Type"/> representing the generic response type (<c>TResponse</c>) of the request.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="request"/> is <c>null</c> or when the provided object does not
    /// implement <see cref="IRequest{TResponse}"/> or <see cref="ICommandOrQuery{TResponse}"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// var command = new CreateUserCommand { Name = "Alice" };
    /// Type responseType = command.GetResponseType();
    ///
    /// Console.WriteLine(responseType.Name); // Output: CreateUserResponse
    /// </code>
    /// </example>
    public static Type GetResponseType(this object request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();

        var iFace = requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType &&
                                 (i.GetGenericTypeDefinition() == typeof(ICommandOrQuery<>) ||
                                  i.GetGenericTypeDefinition() == typeof(IRequest<>)));

        return iFace is null
            ? throw new ArgumentNullException($"Request {requestType.Name} does not implement IRequest<>")
            : iFace.GetGenericArguments()[0];
    }
}
