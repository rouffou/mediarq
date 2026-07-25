using System.Security.Claims;
using FluentAssertions;
using Mediarq.Authorization.Tests.Fixtures;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Mediarq.Authorization.Tests;

public class AuthorizationBehaviorTests
{
    private static ClaimsPrincipal AuthenticatedPrincipal() =>
        new(new ClaimsIdentity([new Claim(ClaimTypes.Name, "alice")], "TestAuth"));

    private static ClaimsPrincipal UnauthenticatedPrincipal() =>
        new(new ClaimsIdentity()); // no authenticationType -> Identity.IsAuthenticated == false

    private static IHttpContextAccessor AccessorFor(ClaimsPrincipal? principal)
    {
        var accessor = new Mock<IHttpContextAccessor>();

        if (principal is null)
        {
            accessor.Setup(a => a.HttpContext).Returns((HttpContext)null!);
        }
        else
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.User).Returns(principal);
            accessor.Setup(a => a.HttpContext).Returns(httpContext.Object);
        }

        return accessor.Object;
    }

    [Fact]
    public void IsActive_Reflects_Whether_The_Request_Type_Implements_IAuthorizedRequest()
    {
        var authService = new Mock<IAuthorizationService>();

        new AuthorizationBehavior<PlainCommand, Result>(authService.Object, AccessorFor(null)).IsActive.Should().BeFalse();
        new AuthorizationBehavior<AuthorizedCommand, Result<string>>(authService.Object, AccessorFor(null)).IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Returns_Unauthorized_Failure_When_No_HttpContext()
    {
        var authService = new Mock<IAuthorizationService>();
        var behavior = new AuthorizationBehavior<AuthorizedCommand, Result<string>>(authService.Object, AccessorFor(null));
        var context = new RequestContext<AuthorizedCommand, Result<string>>(new AuthorizedCommand(null), "user");

        var result = await behavior.Handle(context, () => Task.FromResult(Result.Success("should not run")));

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Handle_Returns_Unauthorized_Failure_When_Principal_Is_Not_Authenticated()
    {
        var authService = new Mock<IAuthorizationService>();
        var behavior = new AuthorizationBehavior<AuthorizedCommand, Result<string>>(authService.Object, AccessorFor(UnauthenticatedPrincipal()));
        var context = new RequestContext<AuthorizedCommand, Result<string>>(new AuthorizedCommand(null), "user");
        var handlerCalled = false;

        var result = await behavior.Handle(context, () =>
        {
            handlerCalled = true;
            return Task.FromResult(Result.Success("should not run"));
        });

        handlerCalled.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        authService.Verify(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object?>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Invokes_Handler_When_Authenticated_And_No_Policy_Is_Required()
    {
        var authService = new Mock<IAuthorizationService>();
        var behavior = new AuthorizationBehavior<AuthorizedCommand, Result<string>>(authService.Object, AccessorFor(AuthenticatedPrincipal()));
        var context = new RequestContext<AuthorizedCommand, Result<string>>(new AuthorizedCommand(null), "user");

        var result = await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
        authService.Verify(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object?>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Invokes_Handler_When_The_Policy_Succeeds()
    {
        var authService = new Mock<IAuthorizationService>();
        authService
            .Setup(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object?>(), "OrdersAdmin"))
            .ReturnsAsync(AuthorizationResult.Success());

        var behavior = new AuthorizationBehavior<AuthorizedCommand, Result<string>>(authService.Object, AccessorFor(AuthenticatedPrincipal()));
        var request = new AuthorizedCommand("OrdersAdmin");
        var context = new RequestContext<AuthorizedCommand, Result<string>>(request, "user");

        var result = await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")));

        result.IsSuccess.Should().BeTrue();
        authService.Verify(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), request, "OrdersAdmin"), Times.Once);
    }

    [Fact]
    public async Task Handle_Returns_Forbidden_Failure_When_The_Policy_Fails()
    {
        var authService = new Mock<IAuthorizationService>();
        authService
            .Setup(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object?>(), "OrdersAdmin"))
            .ReturnsAsync(AuthorizationResult.Failed());

        var behavior = new AuthorizationBehavior<AuthorizedCommand, Result<string>>(authService.Object, AccessorFor(AuthenticatedPrincipal()));
        var context = new RequestContext<AuthorizedCommand, Result<string>>(new AuthorizedCommand("OrdersAdmin"), "user");
        var handlerCalled = false;

        var result = await behavior.Handle(context, () =>
        {
            handlerCalled = true;
            return Task.FromResult(Result.Success("should not run"));
        });

        handlerCalled.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }

    [Fact]
    public async Task Handle_Builds_A_Result_Of_T_Failure_Via_The_Reflection_Fallback()
    {
        var authService = new Mock<IAuthorizationService>();
        var behavior = new AuthorizationBehavior<AuthorizedQuery, Result<int>>(authService.Object, AccessorFor(null));
        var context = new RequestContext<AuthorizedQuery, Result<int>>(new AuthorizedQuery(null), "user");

        var result = await behavior.Handle(context, () => Task.FromResult(Result.Success(42)));

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Handle_Throws_When_TResponse_Is_Not_A_Supported_Result_Type()
    {
        var authService = new Mock<IAuthorizationService>();
        var behavior = new AuthorizationBehavior<AuthorizedRawStringCommand, string>(authService.Object, AccessorFor(null));
        var context = new RequestContext<AuthorizedRawStringCommand, string>(new AuthorizedRawStringCommand(null), "user");

        var act = () => behavior.Handle(context, () => Task.FromResult("should not run"));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public void Constructor_Throws_On_Null_Arguments()
    {
        var authService = new Mock<IAuthorizationService>();
        var accessor = AccessorFor(null);

        ((Action)(() => new AuthorizationBehavior<PlainCommand, Result>(null!, accessor))).Should().Throw<ArgumentNullException>();
        ((Action)(() => new AuthorizationBehavior<PlainCommand, Result>(authService.Object, null!))).Should().Throw<ArgumentNullException>();
    }
}
