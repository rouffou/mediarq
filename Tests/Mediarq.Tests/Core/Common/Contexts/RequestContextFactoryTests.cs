using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Common.Time;
using Mediarq.Core.Common.User;
using Mediarq.Tests.Data;
using Moq;

namespace Mediarq.Tests.Core.Common.Contexts;
public class RequestContextFactoryTests
{
    private readonly RequestContextFactory _testClass;
    private readonly Mock<IUserContext> _userContext;

    public RequestContextFactoryTests()
    {
        _userContext = new Mock<IUserContext>();
        _testClass = new RequestContextFactory(_userContext.Object);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new RequestContextFactory(_userContext.Object);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullUserContext()
    {
        FluentActions.Invoking(() => new RequestContextFactory(default(IUserContext))).Should().Throw<ArgumentNullException>().WithParameterName("userContext");
    }

    [Fact]
    public void CanCallCreate()
    {
        // Arrange
        var request = new TestCommandWithValue(string.Empty); // On utilise un type qui implémente ICommandOrQuery<TResponse>
        var cancellationToken = CancellationToken.None;
        var userId = "TestUser123";
        _userContext.Setup(x => x.UserId).Returns(userId);

        // Act
        var result = _testClass.Create<TestCommandWithValue, Result<string>>(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(RequestContext<TestCommandWithValue, Result<string>>));

        var context = (RequestContext<TestCommandWithValue, Result<string>>)result;
        context.Request.Should().Be(request);
        context.UserId.Should().Be(userId);
        context.CancellationToken.Should().Be(cancellationToken);
        context.RequestId.Should().NotBe(Guid.Empty);
        context.StartedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }
}