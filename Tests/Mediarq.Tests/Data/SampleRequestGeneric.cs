using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.Core.Tests.Common.Pipeline.Behaviors
{
    public partial class ValidationBehaviorTests
    {
        private class SampleRequestGeneric : ICommand<Result<string>>
        {
            public string Data { get; set; } = string.Empty;
        }
    }
}
