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
        // Arrange
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<TestCommand>()))
            .Returns(new[] { ValidationResult.Failure(new[] { new ValidationPropertyError("ValidationError", "Invalid data") }) });
        
        var validationBehavior = new ValidationBehavior<TestCommand, Result>(new[] { validatorMock.Object });
        
        var requestContextMock = new Mock<IIMMutableRequestContext<TestCommand, Result>>();
        requestContextMock.SetupGet(r => r.Request).Returns(new TestCommand("test"));

        // Act
        var result = await validationBehavior.Handle(requestContextMock.Object, () => Task.FromResult(Result.Success()));
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("One or More validation errors occurred");
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().StartWith("Validation.General");
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldProceedToNextBehavior()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestCommand>>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<TestCommand>()))
            .Returns(Array.Empty<ValidationResult>());
        
        var validationBehavior = new ValidationBehavior<TestCommand, Result>(new[] { validatorMock.Object });
        
        var requestContextMock = new Mock<IIMMutableRequestContext<TestCommand, Result>>();
        requestContextMock.SetupGet(r => r.Request).Returns(new TestCommand("test"));
        
        // Act
        var result = await validationBehavior.Handle(requestContextMock.Object, () => Task.FromResult(Result.Success()));
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
    }
}
