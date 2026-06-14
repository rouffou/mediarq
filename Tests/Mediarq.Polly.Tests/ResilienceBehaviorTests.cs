using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Polly;
using Microsoft.Extensions.DependencyInjection;
using global::Polly;
using global::Polly.Registry;
using global::Polly.Retry;

namespace Mediarq.Polly.Tests;

public class ResilienceBehaviorTests
{
    public record FlakyCommand : ICommand<Result<string>>, IResilientRequest
    {
        public string ResiliencePipelineName => "retry";
    }

    public record PlainCommand : ICommand<Result<string>>;

    private static ResiliencePipelineProvider<string> BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddResiliencePipeline("retry", builder => builder.AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.Zero,
            ShouldHandle = new PredicateBuilder().Handle<InvalidOperationException>(),
        }));

        return services.BuildServiceProvider().GetRequiredService<ResiliencePipelineProvider<string>>();
    }

    [Fact]
    public async Task Retries_Transient_Failures_Until_Success()
    {
        var behavior = new ResilienceBehavior<FlakyCommand, Result<string>>(BuildProvider());
        var context = new RequestContext<FlakyCommand, Result<string>>(new FlakyCommand(), "user");

        var attempts = 0;
        var result = await behavior.Handle(context, () =>
        {
            attempts++;
            if (attempts < 3)
            {
                throw new InvalidOperationException("transient");
            }

            return Task.FromResult(Result.Success("ok"));
        });

        attempts.Should().Be(3);
        result.Value.Should().Be("ok");
    }

    [Fact]
    public async Task PassesThrough_NonResilient_Request()
    {
        var behavior = new ResilienceBehavior<PlainCommand, Result<string>>(BuildProvider());
        var context = new RequestContext<PlainCommand, Result<string>>(new PlainCommand(), "user");

        var attempts = 0;
        await behavior.Handle(context, () =>
        {
            attempts++;
            return Task.FromResult(Result.Success("ok"));
        });

        attempts.Should().Be(1);
    }

    [Fact]
    public void AddMediarqResilience_Registers_Behavior()
    {
        var services = new ServiceCollection();

        services.AddMediarqResilience();

        services.Should().Contain(d => d.ServiceType == typeof(IPipelineBehavior<,>)
            && d.ImplementationType == typeof(ResilienceBehavior<,>));
    }
}
