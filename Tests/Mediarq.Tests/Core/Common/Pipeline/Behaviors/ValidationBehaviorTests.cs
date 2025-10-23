using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline.Behaviors;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;
using Mediarq.Tests.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Mediarq.Core.Tests.Common.Pipeline.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<IIMMutableRequestContext<TestCommand, Result>> _mockContextResult;
    private readonly Mock<IIMMutableRequestContext<TestCommandWithValue, Result<string>>> _mockContextResultT;

    public ValidationBehaviorTests()
    {
        _mockContextResult = new Mock<IIMMutableRequestContext<TestCommand, Result>>();
        _mockContextResultT = new Mock<IIMMutableRequestContext<TestCommandWithValue, Result<string>>>();
    }

    [Fact]
    public async Task Handle_Should_CallNext_When_NoValidators()
    {
        // Arrange
        var nextCalled = false;
        var validators = Enumerable.Empty<IValidator<TestCommand>>();
        var behavior = new ValidationBehavior<TestCommand, Result>(validators);
        var command = new TestCommand("OK");

        _mockContextResult.SetupGet(c => c.Request).Returns(command);

        // Act
        var response = await behavior.Handle(_mockContextResult.Object, () =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Success());
        });

        // Assert
        nextCalled.Should().BeTrue();
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_CallNext_When_ValidatorsReturnValid()
    {
        // Arrange
        var nextCalled = false;
        var mockValidator = new Mock<IValidator<TestCommand>>();
        mockValidator
            .Setup(v => v.Validate(It.IsAny<TestCommand>()))
            .Returns(new[]
            {
                ValidationResult.Success()
            });

        var behavior = new ValidationBehavior<TestCommand, Result>(new[] { mockValidator.Object });
        var command = new TestCommand("Valid");
        _mockContextResult.SetupGet(c => c.Request).Returns(command);

        // Act
        var response = await behavior.Handle(_mockContextResult.Object, () =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Success());
        });

        // Assert
        nextCalled.Should().BeTrue();
        response.IsSuccess.Should().BeTrue();
        response.Error.Should().BeEquivalentTo(ResultError.None);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ValidationFails_For_Result()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<TestCommand>>();
        mockValidator
            .Setup(v => v.Validate(It.IsAny<TestCommand>()))
            .Returns(new[]
            {
                ValidationResult.Failure(new[]
                {
                    new ValidationPropertyError("Name", "Name is required")
                })
            });

        var behavior = new ValidationBehavior<TestCommand, Result>(new[] { mockValidator.Object });
        var command = new TestCommand(string.Empty);
        _mockContextResult.SetupGet(c => c.Request).Returns(command);

        // Act
        var response = await behavior.Handle(_mockContextResult.Object, () =>
            Task.FromResult(Result.Success()));

        // Assert
        response.Should().NotBeNull();
        response.IsFailure.Should().BeTrue();
        response.Error.Should().BeOfType<ValidationError>();
        var validationError = (ValidationError)response.Error;
        validationError.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Name is required");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ValidationFails_For_ResultT()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<TestCommandWithValue>>();
        mockValidator
            .Setup(v => v.Validate(It.IsAny<TestCommandWithValue>()))
            .Returns(new[]
            {
                ValidationResult.Failure(new[]
                {
                    new ValidationPropertyError("Name", "Invalid Name")
                })
            });

        var behavior = new ValidationBehavior<TestCommandWithValue, Result<string>>(new[] { mockValidator.Object });
        var command = new TestCommandWithValue("BadName");
        _mockContextResultT.SetupGet(c => c.Request).Returns(command);

        // Act
        var response = await behavior.Handle(_mockContextResultT.Object, () =>
            Task.FromResult(Result.Success(Guid.NewGuid().ToString())));

        // Assert
        response.Should().NotBeNull();
        response.IsFailure.Should().BeTrue();
        response.Error.Should().BeOfType<ValidationError>();
    }

    [Fact]
    public async Task Handle_Should_Throw_When_TResponse_NotAResultType()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<TestCommandWithVReturnUnsupported>>();
        mockValidator
            .Setup(v => v.Validate(It.IsAny<TestCommandWithVReturnUnsupported>()))
            .Returns(new[]
            {
                ValidationResult.Failure(new[]
                {
                    new ValidationPropertyError("Field", "Error")
                })
            });

        var behavior = new ValidationBehavior<TestCommandWithVReturnUnsupported, string>(new[] { mockValidator.Object });
        var command = new TestCommandWithVReturnUnsupported("Test");
        var mockContext = new Mock<IIMMutableRequestContext<TestCommandWithVReturnUnsupported, string>>();
        mockContext.SetupGet(c => c.Request).Returns(command);

        // Act
        var act = async () => await behavior.Handle(mockContext.Object, () => Task.FromResult("OK"));

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*not a supported Result type*");
    }
}
