namespace Mediarq.Tests.Core.Common.Results
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Mediarq.Core.Common.Results;
    using Xunit;

    public class ValidationErrorTests
    {
        [Fact]
        public void Should_Contain_All_Inner_Errors()
        {
            // Arrange
            var innerErrors = new List<Error>
            {
                Error.Failure("Field1", "Error message 1"),
                Error.Failure("Field2", "Error message 2"),
                Error.Failure("Field3", "Error message 3")
            };

            // Act
            var validationError = new ValidationError(innerErrors.ToArray());

            // Assert
            validationError.Errors.Should().NotBeNull();
            validationError.Errors.Should().HaveCount(3);
            validationError.Errors.Should().BeEquivalentTo(innerErrors);
        }

        [Fact]
        public void Should_Handle_Empty_Result()
        {
            // Arrange
            var results = Array.Empty<Result>();
            // Act
            var validationError = ValidationError.FromResults(results);
            // Assert
            validationError.Errors.Should().NotBeNull();
            validationError.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Should_Create_ValidationError_From_Results()
        {
            // Arrange
            var results = new List<Result>
            {
                Result.Success(),
                Result.Failure(Error.Failure("Field1", "Error message 1")),
                Result.Failure(Error.Failure("Field2", "Error message 2")),
                Result.Success(),
                Result.Failure(Error.Failure("Field3", "Error message 3"))
            };
            
            // Act
            var validationError = ValidationError.FromResults(results);

            // Assert
            validationError.Errors.Should().NotBeNull();
            validationError.Errors.Should().HaveCount(3);
            validationError.Errors.Should().Contain(e => e.Code == "Field1" && e.Message == "Error message 1");
            validationError.Errors.Should().Contain(e => e.Code == "Field2" && e.Message == "Error message 2");
            validationError.Errors.Should().Contain(e => e.Code == "Field3" && e.Message == "Error message 3");
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_Errors_Is_Null()
        {            
            // Act
            Action act = () => new ValidationError(null);
            
            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*errors*");
        }
    }
}