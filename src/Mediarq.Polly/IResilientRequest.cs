namespace Mediarq.Polly;

/// <summary>
/// Marks a request that should be executed through a named Polly resilience pipeline (for example
/// retry, timeout, circuit breaker). Configure the pipeline with
/// <c>services.AddResiliencePipeline(name, ...)</c>.
/// </summary>
public interface IResilientRequest
{
    /// <summary>Gets the name of the Polly resilience pipeline to apply to this request.</summary>
    string ResiliencePipelineName { get; }
}
