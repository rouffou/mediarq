namespace Mediarq.Core.Common.User;


/// <summary>
/// Represents contextual information about the currently authenticated user.
/// </summary>
/// <remarks>
/// This interface provides essential user-related data such as the user identifier, 
/// username, and assigned roles. It is typically used within application services, 
/// pipelines, or domain logic to perform authorization checks, auditing, or 
/// personalized data access.
/// 
/// Implementations may retrieve user information from an authentication token, 
/// HTTP context, or any custom identity provider.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// public class AuditService
/// {
///     private readonly IUserContext _userContext;
///
///     public AuditService(IUserContext userContext)
///     {
///         _userContext = userContext;
///     }
///
///     public void LogAction(string action)
///     {
///         Console.WriteLine($"{_userContext.UserName} performed {action}");
///     }
/// }
/// </code>
/// </example>
public interface IUserContext
{
    /// <summary>
    /// Gets the unique identifier of the currently authenticated user.
    /// </summary>
    /// <value>
    /// A string representing the user's unique ID (for example, a GUID or system-specific identifier).
    /// </value>
    string UserId { get; }

    /// <summary>
    /// Gets the display name or username of the currently authenticated user.
    /// </summary>
    /// <value>
    /// A string containing the username used for identification or display purposes.
    /// </value>
    string UserName { get; }

    /// <summary>
    /// Gets the collection of roles assigned to the user.
    /// </summary>
    /// <value>
    /// An enumerable list of strings representing the user's roles or permissions within the system.
    /// </value>
    IEnumerable<string> Roles { get; }
}
