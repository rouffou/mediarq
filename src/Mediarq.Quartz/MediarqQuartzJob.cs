using System.Text.Json;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Mediators;
using Quartz;

namespace Mediarq.Quartz;

/// <summary>
/// The Quartz job type <see cref="QuartzSchedulingExtensions"/> schedules. Not meant to be scheduled
/// directly — go through <see cref="QuartzSchedulingExtensions"/>, which builds the job data carrying the
/// command's type and JSON payload that this job reads back on <see cref="Execute"/>.
/// </summary>
internal sealed class MediarqQuartzJob(ISender sender) : IJob
{
    internal const string CommandTypeKey = "mediarq:commandType";
    internal const string CommandPayloadKey = "mediarq:commandPayload";

    public async Task Execute(IJobExecutionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var map = context.JobDetail.JobDataMap;
        var typeName = map.GetString(CommandTypeKey)
            ?? throw new InvalidOperationException($"Job data is missing '{CommandTypeKey}'.");
        var payload = map.GetString(CommandPayloadKey)
            ?? throw new InvalidOperationException($"Job data is missing '{CommandPayloadKey}'.");

        var commandType = Type.GetType(typeName, throwOnError: true)!;
        var command = (ICommand)JsonSerializer.Deserialize(payload, commandType)!;

        await sender.Send(command, context.CancellationToken).ConfigureAwait(false);
    }
}
