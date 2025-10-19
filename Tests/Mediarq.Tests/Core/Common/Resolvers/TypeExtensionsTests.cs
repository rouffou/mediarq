using FluentAssertions;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Resolvers;

namespace Mediarq.Tests.Core.Common.Resolvers;

public class TypeResponseExtensionsTests
{
    // Classe de test implémentant ICommandOrQuery<T>
    private class TestQuery : ICommandOrQuery<string> { }

    [Fact]
    public void CanCallGetResponseType_WhenImplementsICommandOrQuery()
    {
        // Arrange
        var request = new TestQuery();

        // Act
        var result = request.GetResponseType();

        // Assert
        result.Should().Be(typeof(string));
    }

    [Fact]
    public void CannotCallGetResponseTypeWithNullRequest()
    {
        // Act
        Action act = () => ((object)null).GetResponseType();

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("request");
    }

    [Fact]
    public void ThrowsWhenRequestDoesNotImplementExpectedInterface()
    {
        // Arrange
        var request = new object();

        // Act
        Action act = () => request.GetResponseType();

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*does not implement IRequest<>*");
    }

    [Fact]
    public void CanCallGetResponseType_WhenImplementsIRequest()
    {
        // Arrange
        var request = new DummyRequest();

        // Act
        var result = request.GetResponseType();

        // Assert
        result.Should().Be(typeof(int));
    }

    private class DummyRequest : IRequest<int> { }
}