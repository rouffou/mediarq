using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline.Behaviors;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;
using Mediarq.Tests.Data;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Mediarq.Core.Tests.Common.Pipeline.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<IMutableRequestContext<TestCommand, Result>> _mockContextResult;
    private readonly Mock<IMutableRequestContext<TestCommandWithValue, Result<string>>> _mockContextResultT;

    public ValidationBehaviorTests()
    {
        _mockContextResult = new Mock<IMutableRequestContext<TestCommand, Result>>();
        _mockContextResultT = new Mock<IMutableRequestContext<TestCommandWithValue, Result<string>>>();
    }

    [Fact]
    public async Task Handle_Should_CallNext_When_NoValidators()
    {
        var nextCalled = false;
        var validators = Enumerable.Empty<IValidator<TestCommand>>();
        var behavior = new ValidationBehavior<TestCommand, Result>(validators);
        _mockContextResult.SetupGet(c => c.Request).Returns(new TestCommand("OK"));

        var response = await behavior.Handle(_mockContextResult.Object, () =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Success());
        });

        nextCalled.Should().BeTrue();
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_CallNext_When_ValidatorsReturnValid()
    {
        var nextCalled = false;
        var mockValidator = new Mock<IValidator<TestCommand>>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { ValidationResult.Success() });

        var behavior = new ValidationBehavior<TestCommand, Result>(new[] { mockValidator.Object });
        _mockContextResult.SetupGet(c => c.Request).Returns(new TestCommand("Valid"));

        var response = await behavior.Handle(_mockContextResult.Object, () =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Success());
        });

        nextCalled.Should().BeTrue();
        response.IsSuccess.Should().BeTrue();
        response.Error.Should().BeEquivalentTo(ResultError.None);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ValidationFails_For_Result()
    {
        var mockValidator = new Mock<IValidator<TestCommand>>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { ValidationResult.Failure(new[] { new ValidationPropertyError("Name", "Name is required") }) });

        var behavior = new ValidationBehavior<TestCommand, Result>(new[] { mockValidator.Object });
        _mockContextResult.SetupGet(c => c.Request).Returns(new TestCommand(string.Empty));

        var response = await behavior.Handle(_mockContextResult.Object, () => Task.FromResult(Result.Success()));

        response.Should().NotBeNull();
        response.IsFailure.Should().BeTrue();
        response.Error.Should().BeOfType<ValidationError>();
        var validationError = (ValidationError)response.Error;
        validationError.Errors.Should().ContainSingle().Which.Message.Should().Contain("Name is required");
    }

    [Fact]
    public async Task Handle_Should_Localize_Message_When_Resolver_Provided()
    {
        var mockValidator = new Mock<IValidator<TestCommand>>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { ValidationResult.Failure(new[] { new ValidationPropertyError("Name", "Name is required") }) });

        var resolver = new Mock<IValidationMessageResolver>();
        resolver.Setup(r => r.Resolve("Name", "Name is required")).Returns("Nom requis");

        var behavior = new ValidationBehavior<TestCommand, Result>(new[] { mockValidator.Object }, resolver.Object);
        _mockContextResult.SetupGet(c => c.Request).Returns(new TestCommand(string.Empty));

        var response = await behavior.Handle(_mockContextResult.Object, () => Task.FromResult(Result.Success()));

        response.IsFailure.Should().BeTrue();
        var validationError = (ValidationError)response.Error;
        validationError.Errors.Should().ContainSingle().Which.Message.Should().Be("Nom requis");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ValidationFails_For_ResultT()
    {
        var mockValidator = new Mock<IValidator<TestCommandWithValue>>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<TestCommandWithValue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { ValidationResult.Failure(new[] { new ValidationPropertyError("Name", "Invalid Name") }) });

        var behavior = new ValidationBehavior<TestCommandWithValue, Result<string>>(new[] { mockValidator.Object });
        _mockContextResultT.SetupGet(c => c.Request).Returns(new TestCommandWithValue("BadName"));

        var response = await behavior.Handle(_mockContextResultT.Object, () => Task.FromResult(Result.Success(Guid.NewGuid().ToString())));

        response.Should().NotBeNull();
        response.IsFailure.Should().BeTrue();
        response.Error.Should().BeOfType<ValidationError>();
    }

    [Fact]
    public async Task Handle_Should_Throw_When_TResponse_NotAResultType()
    {
        var mockValidator = new Mock<IValidator<TestCommandWithVReturnUnsupported>>();
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<TestCommandWithVReturnUnsupported>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { ValidationResult.Failure(new[] { new ValidationPropertyError("Field", "Error") }) });

        var behavior = new ValidationBehavior<TestCommandWithVReturnUnsupported, string>(new[] { mockValidator.Object });
        var mockContext = new Mock<IMutableRequestContext<TestCommandWithVReturnUnsupported, string>>();
        mockContext.SetupGet(c => c.Request).Returns(new TestCommandWithVReturnUnsupported("Test"));

        var act = async () => await behavior.Handle(mockContext.Object, () => Task.FromResult("OK"));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not a supported Result type*");
    }
}
