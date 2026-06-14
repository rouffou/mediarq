namespace Mediarq.Core.Mediators;

/// <summary>
/// Central entry point for dispatching application requests (commands or queries) and publishing
/// notifications. Combines <see cref="ISender"/> and <see cref="IPublisher"/>.
/// </summary>
/// <remarks>
/// The mediator decouples the sending component (e.g. controllers, services) from the handling
/// component (command/query/notification handlers), and integrates pipeline behaviors such as
/// logging, validation and performance tracking.
///
/// Depend on <see cref="ISender"/> or <see cref="IPublisher"/> directly when a component only needs
/// one of the two responsibilities.
/// </remarks>
/// <example>
/// <code>
/// public class CreateUserCommand : ICommand&lt;Result&lt;Guid&gt;&gt; { public string Name { get; set; } }
///
/// var result = await mediator.Send(new CreateUserCommand { Name = "Alice" });
/// </code>
/// </example>
public interface IMediator : ISender, IPublisher;
