using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Tests.Common.Pipeline.Behaviors
{
    public record DummyContext<TRequest, TResponse>(TRequest request) : IMutableRequestContext<TRequest, TResponse>
        where TRequest : ICommandOrQuery<TResponse>
    {
        public Guid RequestId { get; } = Guid.NewGuid();
        public string UserId { get; } = "TestUser";
        public DateTime StartedAt { get; } = DateTime.UtcNow;
        public DateTime FinishedAt { get; set; } = DateTime.MinValue;
        public TRequest Request { get; } = request;
        public CancellationToken CancellationToken { get; } = CancellationToken.None;

        private readonly Dictionary<string, object> _items = new();
        public IReadOnlyDictionary<string, object> Items => _items;

        public void AddItem(string key, object value) => _items[key] = value;
    }
}
