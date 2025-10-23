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
public record DefaultUserContext : IUserContext
{
    /// <summary>
    /// Gets the unique identifier of the user.
    /// </summary>
    /// <value>
    /// A string representing the user's unique ID.
    /// Defaults to <see cref="string.Empty"/> if not provided.
    /// </value>
    public string UserId { get; init; }

    /// <summary>
    /// Gets the username or display name of the user.
    /// </summary>
    /// <value>
    /// A string representing the user's name.
    /// Defaults to <c>"system"</c> when no username is specified.
    /// </value>
    public string UserName { get; init; }

    /// <summary>
    /// Gets the list of roles assigned to the user.
    /// </summary>
    /// <value>
    /// An enumerable collection of role names.
    /// Defaults to an empty collection when no roles are provided.
    /// </value>
    public IEnumerable<string> Roles { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultUserContext"/> class.
    /// </summary>
    /// <param name="userId">The unique identifier of the user. Defaults to an empty string.</param>
    /// <param name="userName">The username of the user. Defaults to <c>"system"</c>.</param>
    /// <param name="roles">The roles associated with the user. Defaults to an empty list.</param>
    public DefaultUserContext(string userId = null, string userName = "system", IEnumerable<string> roles = null)
    {
        UserId = userId ?? string.Empty;
        UserName = userName;
        Roles = roles ?? [];
    }
}
