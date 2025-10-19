using FluentAssertions;
using Mediarq.Core.Common.User;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Mediarq.Tests.Core.Common.User;
public class HttpUserContextTests
{
    private readonly HttpUserContext _testClass;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

    public HttpUserContextTests()
    {
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _testClass = new HttpUserContext(_httpContextAccessor.Object);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new HttpUserContext(_httpContextAccessor.Object);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullHttpContextAccessor()
    {
        FluentActions.Invoking(() => new HttpUserContext(default(IHttpContextAccessor)))
            .Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("httpContextAccessor");
    }

    [Fact]
    public void CanGetUserId()
    {
        // Arrange
        var userId = "12345";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "mock");
        var user = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = user };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        // Act
        var result = _testClass.UserId;

        // Assert
        result.Should().Be(userId);
    }

    [Fact]
    public void CanGetUserName()
    {
        // Arrange
        var userName = "TestUser";
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "mock");
        var user = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = user };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        // Act
        var result = _testClass.UserName;

        // Assert
        result.Should().Be(userName);
    }

    [Fact]
    public void CanGetRoles()
    {
        // Arrange
        var roles = new[] { "Admin", "User" };
        var claims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
        var identity = new ClaimsIdentity(claims, "mock");
        var user = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = user };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        // Act
        var result = _testClass.Roles;

        // Assert
        result.Should().BeEquivalentTo(roles);
    }

    [Fact]
    public void ReturnsEmptyRoles_WhenNoRolesPresent()
    {
        // Arrange
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) };
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        // Act
        var result = _testClass.Roles;

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ReturnsNull_WhenHttpContextIsNull()
    {
        // Arrange
        _httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null);

        // Act & Assert
        _testClass.UserId.Should().BeNull();
        _testClass.UserName.Should().BeNull();
        _testClass.Roles.Should().BeEmpty();
    }
}