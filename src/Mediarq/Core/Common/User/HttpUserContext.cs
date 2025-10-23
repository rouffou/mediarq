using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Mediarq.Core.Common.User;

/// <summary>
/// Provides an implementation of the <see cref="IUserContext"/> interface 
/// that retrieves user information from the current HTTP context.
/// </summary>
/// <remarks>
/// This class is designed for ASP.NET Core applications where user data 
/// is available through <see cref="HttpContext.User"/>. It extracts the 
/// user’s identifier, username, and roles from the claims principal.
///
/// When no user is authenticated or when <see cref="HttpContext"/> is unavailable,
/// the returned values will be <see langword="null"/> or an empty collection.
/// </remarks>
/// <example>
/// Example usage in dependency injection:
/// <code>
/// services.AddHttpContextAccessor();
/// services.AddScoped&lt;IUserContext, HttpUserContext&gt;();
///
/// // Usage inside an application service:
/// public class AccountService
/// {
///     private readonly IUserContext _userContext;
///
///     public AccountService(IUserContext userContext)
///     {
///         _userContext = userContext;
///     }
///
///     public void PrintCurrentUser()
///     {
///         Console.WriteLine($"User: {_userContext.UserName} (ID: {_userContext.UserId})");
///     }
/// }
/// </code>
/// </example>
public class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpUserContext"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">
    /// The <see cref="IHttpContextAccessor"/> used to access the current <see cref="HttpContext"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpContextAccessor"/> is <see langword="null"/>.
    /// </exception>
    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the unique identifier of the authenticated user.
    /// </summary>
    /// <value>
    /// The value of the <see cref="ClaimTypes.NameIdentifier"/> claim,
    /// or <see langword="null"/> if the user is not authenticated.
    /// </value>
    public string UserId
        => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// Gets the username of the authenticated user.
    /// </summary>
    /// <value>
    /// The value of the <see cref="ClaimsIdentity.Name"/> property,
    /// or <see langword="null"/> if the user is not authenticated.
    /// </value>
    public string UserName
        => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    /// <summary>
    /// Gets the roles associated with the authenticated user.
    /// </summary>
    /// <value>
    /// A collection of role names extracted from claims of type <see cref="ClaimTypes.Role"/>.
    /// Returns an empty collection if the user has no roles or is not authenticated.
    /// </value>
    public IEnumerable<string> Roles
        => _httpContextAccessor.HttpContext?.User?
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value) ?? [];
}
