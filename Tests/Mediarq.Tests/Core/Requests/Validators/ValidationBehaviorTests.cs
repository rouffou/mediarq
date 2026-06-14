using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline.Behaviors;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;
using Mediarq.Tests.Data;
using Moq;

namespace Mediarq.Tests.Core.Requests.Validators;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_InvalidRequest_ShouldReturnFailureResult()
    {
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { ValidationResult.Failure(new[] { new ValidationPropertyError("ValidationError", "Invalid data") }) });

        var validationBehavior = new ValidationBehavior<TestCommand, Result>(new[] { validatorMock.Object });

        var requestContextMock = new Mock<IMutableRequestContext<TestCommand, Result>>();
        requestContextMock.SetupGet(r => r.Request).Returns(new TestCommand("test"));

        var result = await validationBehavior.Handle(requestContextMock.Object, () => Task.FromResult(Result.Success()));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("One or More validation errors occurred");
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().StartWith("Validation.General");
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldProceedToNextBehavior()
    {
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ValidationResult>());

        var validationBehavior = new ValidationBehavior<TestCommand, Result>(new[] { validatorMock.Object });

        var requestContextMock = new Mock<IMutableRequestContext<TestCommand, Result>>();
        requestContextMock.SetupGet(r => r.Request).Returns(new TestCommand("test"));

        var result = await validationBehavior.Handle(requestContextMock.Object, () => Task.FromResult(Result.Success()));

        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(ResultError.None);
    }
}
