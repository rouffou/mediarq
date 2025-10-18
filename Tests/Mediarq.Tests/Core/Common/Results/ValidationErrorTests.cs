namespace Mediarq.Tests.Core.Common.Results
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Mediarq.Core.Common.Results;
    using Xunit;

    public class ValidationErrorTests
    {
        private readonly ValidationError _testClass;
        private Error[] _errors;

        public ValidationErrorTests()
        {
            _errors = new[] { new Error("TestValue1429113625", "TestValue1142359139", ErrorType.Validation), new Error("TestValue2018436828", "TestValue966419306", ErrorType.Failure), new Error("TestValue1192515544", "TestValue1534665964", ErrorType.Problem) };
            _testClass = new ValidationError(_errors);
        }

        [Fact]
        public void CanConstruct()
        {
            // Act
            var instance = new ValidationError(_errors);

            // Assert
            instance.Should().NotBeNull();
        }

        [Fact]
        public void CanCallFromResults()
        {
            // Arrange
            var results = new[] { Result.Success(), Result.Success(), Result.Success() };

            // Act
            var result = ValidationError.FromResults(results);

            // Assert
            //throw new NotImplementedException("Create or modify test");
        }

        [Fact]
        public void CannotCallFromResultsWithNullResults()
        {
            FluentActions.Invoking(() => ValidationError.FromResults(default)).Should().Throw<ArgumentNullException>().WithParameterName("results");
        }

        [Fact]
        public void ErrorsIsInitializedCorrectly()
        {
            _testClass.Errors.Should().BeSameAs(_errors);
        }
    }
}