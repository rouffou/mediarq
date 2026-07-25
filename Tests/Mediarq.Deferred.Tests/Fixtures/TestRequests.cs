using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Deferred.Tests.Fixtures;

public sealed record TestCommand(string Payload) : ICommand;

public sealed record TestNotification(string Payload) : INotification;
