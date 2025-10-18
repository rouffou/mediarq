using Mediarq.Core.Common.Pipeline.Behaviors;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Mediarq.Core.Tests.Common.Pipeline.Behaviors
{
    public partial class ValidationBehaviorTests
    {

        // 🧪 1. Aucun validateur → next() est appelé
        [Fact]
        public async Task Handle_NoValidators_CallsNext()
        {
            // Arrange
            var behavior = new ValidationBehavior<SampleRequest, Result>(Enumerable.Empty<IValidator<SampleRequest>>());
            var context = new DummyContext<SampleRequest, Result>(new SampleRequest());
            bool nextCalled = false;

            Task<Result> Next()
            {
                nextCalled = true;
                return Task.FromResult(Result.Success());
            }

            // Act
            var result = await behavior.Handle(context, Next);

            // Assert
            Assert.True(nextCalled);
            Assert.True(result.IsSuccess);
        }

        // 🧪 2. Validateurs sans erreurs → next() est appelé
        [Fact]
        public async Task Handle_ValidatorsWithNoErrors_CallsNext()
        {
            // Arrange
            var validator = new DummyValidator<SampleRequest>(new List<ValidationResult>
            {
                new ValidationResult(true, Array.Empty<ValidationPropertyError>())
            });

            var behavior = new ValidationBehavior<SampleRequest, Result>(new[] { validator });
            var context = new DummyContext<SampleRequest, Result>(new SampleRequest());
            bool nextCalled = false;

            Task<Result> Next()
            {
                nextCalled = true;
                return Task.FromResult(Result.Success());
            }

            // Act
            var result = await behavior.Handle(context, Next);

            // Assert
            Assert.True(nextCalled);
            Assert.True(result.IsSuccess);
        }

        // 🧪 3. Validateurs avec erreurs → retourne Result.Failure()
        [Fact]
        public async Task Handle_WithValidationErrors_ReturnsFailureResult()
        {
            // Arrange
            var errors = new[]
            {
                new ValidationPropertyError("Name", "Name is required")
            };

            var validator = new DummyValidator<SampleRequest>(new List<ValidationResult>
            {
                new ValidationResult(false, errors)
            });

            var behavior = new ValidationBehavior<SampleRequest, Result>(new[] { validator });
            var context = new DummyContext<SampleRequest, Result>(new SampleRequest());

            // Act
            var result = await behavior.Handle(context, () => Task.FromResult(Result.Success()));

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorType.Validation, result.Error!.Type);
            Assert.Contains("Validation.SampleRequest.Name", result.Error.Code);
        }

        // 🧪 4. Validateurs avec erreurs → retourne Result<T>.ValidationFailure()
        [Fact]
        public async Task Handle_WithValidationErrors_ReturnsFailureGenericResult()
        {
            // Arrange
            var errors = new[]
            {
                new ValidationPropertyError("Data", "Data cannot be empty")
            };

            var validator = new DummyValidator<SampleRequestGeneric>(new List<ValidationResult>
            {
                new ValidationResult(false, errors)
            });

            var behavior = new ValidationBehavior<SampleRequestGeneric, Result<string>>(new[] { validator });
            var context = new DummyContext<SampleRequestGeneric, Result<string>>(new SampleRequestGeneric());

            // Act
            var result = await behavior.Handle(context, () => Task.FromResult(Result.Success("OK")));

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorType.Validation, result.Error!.Type);
            Assert.Contains("Validation.SampleRequestGeneric.Data", result.Error.Code);
        }

        // 🧪 5. TResponse n’est pas un Result → InvalidOperationException
        private class InvalidResponseRequest : ICommand<int> { }

        [Fact]
        public async Task Handle_WithInvalidResponseType_ThrowsInvalidOperationException()
        {
            // Arrange
            var errors = new[]
            {
                new ValidationPropertyError("Id", "Invalid value")
            };

            var validator = new DummyValidator<InvalidResponseRequest>(new List<ValidationResult>
            {
                new ValidationResult(false, errors)
            });

            var behavior = new ValidationBehavior<InvalidResponseRequest, int>(new[] { validator });
            var context = new DummyContext<InvalidResponseRequest, int>(new InvalidResponseRequest());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => behavior.Handle(context, () => Task.FromResult(1)));
        }
    }
}
