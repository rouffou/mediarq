using FluentAssertions;
using Mediarq.Core.Common.Requests.Validators;

namespace Mediarq.Tests.Core.Common.Requests.Validators;
public class ValidationResultTests
{
    private readonly ValidationResult _testClass;
    private IEnumerable<ValidationPropertyError> _errors;

    public ValidationResultTests()
    {
        _errors = new[] { new ValidationPropertyError("TestValue953972516", "TestValue1468914051"), new ValidationPropertyError("TestValue1183674608", "TestValue1740856385"), new ValidationPropertyError("TestValue1000377255", "TestValue1504165492") };
        _testClass = new ValidationResult(_errors);
    }

    [Fact]
    public void CanConstruct()
    {
        // Act
        var instance = new ValidationResult(_errors);

        // Assert
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CanCallSuccess()
    {
        // Act
        var result = ValidationResult.Success();

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void CanCallFailure()
    {
        // Arrange
        var errors = new[] { new ValidationPropertyError("TestValue350899696", "TestValue1350961134"), new ValidationPropertyError("TestValue1172176754", "TestValue1503090486"), new ValidationPropertyError("TestValue2079216927", "TestValue2143473385") };

        // Act
        var result = ValidationResult.Failure(errors);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void CannotCallFailureWithNullErrors()
    {
        FluentActions.Invoking(() => ValidationResult.Failure(null)).Should().Throw<ArgumentNullException>().WithParameterName("errors");
    }

    [Fact]
    public void FailurePerformsMapping()
    {
        // Arrange
        var errors = new[] { new ValidationPropertyError("TestValue774875816", "TestValue1472759321"), new ValidationPropertyError("TestValue787127186", "TestValue1127370408"), new ValidationPropertyError("TestValue505170417", "TestValue1178457304") };

        // Act
        var result = ValidationResult.Failure(errors);

        // Assert
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ErrorsIsInitializedCorrectly()
    {
        _testClass.Errors.Should().BeEquivalentTo(_errors);
    }
}