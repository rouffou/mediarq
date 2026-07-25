namespace Mediarq.Authorization;

/// <summary>
/// Marks a request that must pass authorization before its handler runs. Requires an authenticated
/// user; when <see cref="PolicyName"/> is set, that ASP.NET Core authorization policy must also succeed.
/// </summary>
public interface IAuthorizedRequest
{
    /// <summary>
    /// The name of the ASP.NET Core authorization policy to evaluate (registered via
    /// <c>services.AddAuthorization(o =&gt; o.AddPolicy(name, ...))</c>), or <see langword="null"/> to
    /// only require an authenticated user.
    /// </summary>
    string? PolicyName { get; }
}
