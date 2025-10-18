using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.Core.Tests.Common.Pipeline.Behaviors
{
    public partial class ValidationBehaviorTests
    {
        private class SampleRequest : ICommand<Result>
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
