using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Mediarq.Core.Common.User;

/// <summary>
/// Provides a default implementation of the <see cref="IUserContext"/> interface.
/// </summary>
/// <remarks>
/// This class represents a simple user context that can be used as a fallback 
/// or default implementation when no authenticated user is available.
/// It is particularly useful in background tasks, system processes, or 
/// testing scenarios where user information is optional.
///
/// By default, it assigns the username <c>"system"</c> and an empty set of roles.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// IUserContext userContext = new DefaultUserContext(
///     userId: "12345",
///     userName: "admin",
///     roles: new[] { "Administrator", "Auditor" });
///
/// Console.WriteLine(userContext.UserName); // Output: admin
/// </code>
/// </example>
public class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId
        => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string UserName
        => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public IEnumerable<string> Roles
        => _httpContextAccessor.HttpContext?.User?
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value) ?? [];
}
