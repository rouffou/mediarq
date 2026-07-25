using FluentAssertions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Results;

namespace Mediarq.Tests.Core.Common.Results;

public class ResultTests
{
    private sealed record SampleNotification(string Text) : INotification;

    private static readonly ResultError SampleFailure = ResultError.Failure("SampleError", "This is a sample error message.");

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
        var error = ResultError.Failure("ImplicitError", "This is an implicit error.");
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

    [Fact]
    public void CascadedNotifications_IsEmpty_ByDefault()
    {
        var result = Result.Success();

        result.CascadedNotifications.Should().BeEmpty();
    }

    [Fact]
    public void WithNotifications_OnResult_AttachesNotifications_AndReturnsSameInstance()
    {
        var result = Result.Success();
        var notification = new SampleNotification("first");

        var returned = result.WithNotifications(notification);

        returned.Should().BeSameAs(result);
        result.CascadedNotifications.Should().ContainSingle().Which.Should().Be(notification);
    }

    [Fact]
    public void WithNotifications_OnResultOfT_ReturnsResultOfT_ForFluentChaining()
    {
        // Compiles only if WithNotifications on Result<T> returns Result<T>, not the base Result.
        Result<int> result = Result.Success(42).WithNotifications(new SampleNotification("cascade"));

        result.Value.Should().Be(42);
        result.CascadedNotifications.Should().ContainSingle().Which.Should().BeOfType<SampleNotification>();
    }

    [Fact]
    public void WithNotifications_CalledMultipleTimes_AccumulatesNotifications()
    {
        var result = Result.Success();

        result.WithNotifications(new SampleNotification("a"));
        result.WithNotifications(new SampleNotification("b"), new SampleNotification("c"));

        result.CascadedNotifications.Should().HaveCount(3);
        result.CascadedNotifications.Select(n => ((SampleNotification)n).Text).Should().Equal("a", "b", "c");
    }

    [Fact]
    public void WithNotifications_WithNoArguments_DoesNothing()
    {
        var result = Result.Success();

        result.WithNotifications();

        result.CascadedNotifications.Should().BeEmpty();
    }

    [Fact]
    public void WithNotifications_NullArgument_ShouldThrowArgumentNullException()
    {
        var result = Result.Success();

        Action act = () => result.WithNotifications(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
