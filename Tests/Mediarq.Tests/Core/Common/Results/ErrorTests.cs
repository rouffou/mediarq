using FluentAssertions;
using Mediarq.Core.Common.Results;

namespace Mediarq.Tests.Core.Common.Results;

public class ErrorTests
{
    [Fact]
    public void Error_None_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var error = Error.None;
        // Assert
        error.Code.Should().Be(string.Empty);
        error.Message.Should().Be(string.Empty);
        error.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public void Error_NullValue_ShouldHaveExpectedValues()
    {
        // Arrange & Act
        var error = Error.NullValue;
        // Assert
        error.Code.Should().Be("General.Null");
        error.Message.Should().Be("Null value was provided");
        error.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public void Error_FactoryMethods_ShouldCreateErrorsFailureTypes()
    {
        // Arrange & Act
        var failureError = Error.Failure("FailCode", "Failure occurred");
        // Assert
        failureError.Type.Should().Be(ErrorType.Failure);
        failureError.Code.Should().Be("FailCode");
        failureError.Message.Should().Be("Failure occurred");
    }

    [Fact]
    public void Error_FactoryMethods_ShouldCreateErrorsNotFoundTypes()
    {
        // Arrange & Act
        var notFoundError = Error.NotFound("NotFoundCode", "Resource not found");
        // Assert
        notFoundError.Type.Should().Be(ErrorType.NotFound);
        notFoundError.Code.Should().Be("NotFoundCode");
        notFoundError.Message.Should().Be("Resource not found");
    }

    [Fact]
    public void Error_FactoryMethods_ShouldCreateErrorsConflictTypes()
    {
        // Arrange & Act
        var conflictError = Error.Conflict("ConflictCode", "Conflict occurred");
        // Assert
        conflictError.Type.Should().Be(ErrorType.Conflict);
        conflictError.Code.Should().Be("ConflictCode");
        conflictError.Message.Should().Be("Conflict occurred");
    }

    [Fact]
    public void Error_FactoryMethods_ShouldCreateErrorsProblemTypes()
    {
        // Arrange & Act
        var problemError = Error.Problem("ProblemCode", "Problem occurred");
        // Assert
        problemError.Type.Should().Be(ErrorType.Problem);
        problemError.Code.Should().Be("ProblemCode");
        problemError.Message.Should().Be("Problem occurred");
    }
}
