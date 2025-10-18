using Mediarq.Core.Common.Requests.Validators;

namespace Mediarq.Core.Tests.Common.Pipeline.Behaviors
{
    public partial class ValidationBehaviorTests
    {
        private class DummyValidator<T> : IValidator<T>
        {
            private readonly List<ValidationResult> _results;

            public DummyValidator(List<ValidationResult> results)
            {
                _results = results;
            }

            public IEnumerable<ValidationResult> Validate(T instance) => _results;
        }
    }
}
