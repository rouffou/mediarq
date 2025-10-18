namespace Mediarq.Core.Common.User;

public record DefaultUserContext : IUserContext
{
    public string UserId { get; init; }

    public string UserName { get; init; }

    public IEnumerable<string> Roles { get; init; }

    public DefaultUserContext(string userId = null, string userName = "system", IEnumerable<string> roles = null)
    {
        UserId = userId ?? string.Empty;
        UserName = userName;
        Roles = roles ?? Enumerable.Empty<string>();
    }
}
