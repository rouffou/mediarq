using Mediarq.Core.Common.User;

namespace Mediarq.Testing;

/// <summary>
/// A settable <see cref="IUserContext"/> for tests. All members default to an unauthenticated user
/// (null <see cref="UserId"/>/<see cref="UserName"/>, no roles); set them to simulate a specific user.
/// </summary>
public sealed class FakeUserContext : IUserContext
{
    /// <inheritdoc />
    public string? UserId { get; set; }

    /// <inheritdoc />
    public string? UserName { get; set; }

    /// <inheritdoc />
    public IEnumerable<string> Roles { get; set; } = [];
}
