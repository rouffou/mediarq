using FluentValidation;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.WebApiTemplate1.Features.Todos;

public sealed record CreateTodoCommand(string Title) : ICommand<Result<Guid>>;

public sealed class CreateTodoHandler(ITodoStore store) : ICommandHandler<CreateTodoCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateTodoCommand request, CancellationToken cancellationToken = default)
    {
        var todo = store.Add(new Todo { Id = Guid.NewGuid(), Title = request.Title });
        return Task.FromResult(Result.Success(todo.Id));
    }
}

/// <summary>FluentValidation validator, bridged into the Mediarq pipeline by AddMediarqFluentValidation().</summary>
public sealed class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}
