using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Query;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.HealthChecks.Tests.Fixtures;

public sealed record PingCommand : ICommand;

public sealed class PingCommandHandler : ICommandHandler<PingCommand>
{
    public Task Handle(PingCommand request, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public sealed class PingCommandExtraHandler : ICommandHandler<PingCommand>
{
    public Task Handle(PingCommand request, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public sealed record CountQuery(int Seed) : IQuery<int>;

public sealed class CountQueryHandler : IQueryHandler<CountQuery, int>
{
    public Task<int> Handle(CountQuery request, CancellationToken cancellationToken = default) => Task.FromResult(request.Seed);
}

public static class Wiring
{
    public static void RegisterPingHandler(IServiceCollection services) =>
        services.AddScoped<IRequestHandler<PingCommand, Unit>, PingCommandHandler>();

    public static void RegisterCountHandler(IServiceCollection services) =>
        services.AddScoped<IRequestHandler<CountQuery, int>, CountQueryHandler>();
}
