using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.MassTransit;

/// <summary>
/// Marks a notification as an integration event — one meant to be published out-of-process on a
/// MassTransit bus (in addition to any in-process handlers), so other services can consume it.
/// </summary>
/// <remarks>
/// Use <c>AddMediarqMassTransitForwarding</c> to register the forwarder for these events. Plain
/// <see cref="INotification"/> types remain in-process unless you forward them explicitly.
/// </remarks>
public interface IIntegrationEvent : INotification;
