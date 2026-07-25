using FluentAssertions;
using Mediarq.Core.Common.User;

namespace Mediarq.Testing.Tests;

public class FakeUserContextTests
{
    [Fact]
    public void Defaults_To_An_Unauthenticated_User()
    {
        IUserContext context = new FakeUserContext();

        context.UserId.Should().BeNull();
        context.UserName.Should().BeNull();
        context.Roles.Should().BeEmpty();
    }

    [Fact]
    public void Properties_Are_Settable()
    {
        var context = new FakeUserContext
        {
            UserId = "u1",
            UserName = "alice",
            Roles = ["Admin", "Auditor"],
        };

        context.UserId.Should().Be("u1");
        context.UserName.Should().Be("alice");
        context.Roles.Should().BeEquivalentTo(["Admin", "Auditor"]);
    }
}
