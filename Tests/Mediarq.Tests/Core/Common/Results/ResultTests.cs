using FluentAssertions;
using Mediarq.Core.Common.Results;

namespace Mediarq.Tests.Core.Common.Results;

public class ResultTests
{
    private static readonly Error SampleFailure = Error.Failure("SampleError", "This is a sample error message.");

    [Fact]
    public void SuccessResult_ShouldHaveIsSuccessTrue_AndIsFailureFalse()
    {
        // Arrange & Act
        var result = Result.Success();
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void FailureResult_ShouldHaveIsSuccessFalse_AndIsFailureTrue()
    {
        // Arrange & Act
        var result = Result.Failure(SampleFailure);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SampleFailure);
        result.Error.Code.Should().Be("SampleError");
        result.Error.Message.Should().Be("This is a sample error message.");
        result.Error.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public void FailureResult_NullError_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Action act = () => Result.Failure(null!);
        act.Should().Throw<ArgumentNullException>().WithMessage("*error*");
    }

    [Fact]
    public void SuccessResult_WithValue_ShouldContainValue()
    {
        // Arrange
        var expectedValue = 42;
        // Act
        var result = Result.Success(expectedValue);
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(expectedValue);
    }

    [Fact]
    public void FailureResult_WithValue_ShouldThrowInvalidOperationException_OnValueAccess()
    {
        // Arrange
        var result = Result.Failure<int>(SampleFailure);
        
        // Act
        Action act = () => { var value = result.Value; };

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SampleFailure);
        act.Should().Throw<InvalidOperationException>().WithMessage("The value of a failure result can't be accessed");
    }

    [Fact]
    public void ImplicitConversion_FromValueToResult_ShouldCreateSuccessResult()
    {
        // Arrange
        var expectedValue = "TestString";
        // Act
        Result<string> result = expectedValue;
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedValue);
    }

    [Fact]
    public void ImplicitConversion_FromErrorToResult_ShouldCreateFailureResult()
    {
        // Arrange
        var error = Error.Failure("ImplicitError", "This is an implicit error.");
        // Act
        Result<string> result = error;
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void AccessingValue_OnFailureResult_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var result = Result.Failure<string>(SampleFailure);
        
        // Act
        Action act = () => { var value = result.Value; };
        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("The value of a failure result can't be accessed");
    }
}
