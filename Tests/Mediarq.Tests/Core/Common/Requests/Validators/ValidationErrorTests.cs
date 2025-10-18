namespace Mediarq.Tests.Core.Common.Requests.Validators
{
    using System;
    using FluentAssertions;
    using Mediarq.Core.Common.Requests.Validators;
    using Xunit;

    public class ValidationPropertyErrorTests
    {
        private readonly ValidationPropertyError _testClass;
        private string _propertyName;
        private string _errorMessage;

        public ValidationPropertyErrorTests()
        {
            _propertyName = "TestValue771111146";
            _errorMessage = "TestValue194504868";
            _testClass = new ValidationPropertyError(_propertyName, _errorMessage);
        }

        [Fact]
        public void CanConstruct()
        {
            // Act
            var instance = new ValidationPropertyError(_propertyName, _errorMessage);

            // Assert
            instance.Should().NotBeNull();
        }

        [Fact]
        public void ImplementsIEquatable_ValidationPropertyError()
        {
            // Arrange
            var same = new ValidationPropertyError(_propertyName, _errorMessage);
            var different = new ValidationPropertyError("TestValue362025845", "TestValue453660862");

            // Assert
            _testClass.Equals(default(object)).Should().BeFalse();
            _testClass.Equals(new object()).Should().BeFalse();
            _testClass.Equals((object)same).Should().BeTrue();
            _testClass.Equals((object)different).Should().BeFalse();
            _testClass.Equals(same).Should().BeTrue();
            _testClass.Equals(different).Should().BeFalse();
            _testClass.GetHashCode().Should().Be(same.GetHashCode());
            _testClass.GetHashCode().Should().NotBe(different.GetHashCode());
            (_testClass == same).Should().BeTrue();
            (_testClass == different).Should().BeFalse();
            (_testClass != same).Should().BeFalse();
            (_testClass != different).Should().BeTrue();
        }

        [Fact]
        public void CanCallToString()
        {
            // Act
            var result = _testClass.ToString();
            var expectedValue = $"{_propertyName}: {_errorMessage}";
            // Assert
            result.Should().Be(expectedValue);
        }

        [Fact]
        public void PropertyNameIsInitializedCorrectly()
        {
            _testClass.PropertyName.Should().Be(_propertyName);
        }

        [Fact]
        public void ErrorMessageIsInitializedCorrectly()
        {
            _testClass.ErrorMessage.Should().Be(_errorMessage);
        }
    }
}