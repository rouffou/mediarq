using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Samples.Models;

namespace Mediarq.Samples.Queries;

public record GetUserByIdQuery(Guid Id): IQuery<Result<User>>;
