using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Mediarq.UnitOfWork.Tests;

public class UnitOfWorkBehaviorTests
{
    public record TransactionalCommand(string Name) : ICommand<Result<string>>, ITransactionalRequest;

    public record PlainCommand(string Name) : ICommand<Result<string>>;

    [Fact]
    public async Task Commits_On_Successful_Transactional_Request()
    {
        var uow = new Mock<IUnitOfWork>();
        var behavior = new UnitOfWorkBehavior<TransactionalCommand, Result<string>>(uow.Object);
        var context = new RequestContext<TransactionalCommand, Result<string>>(new TransactionalCommand("a"), "user");

        await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")));

        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Does_Not_Commit_On_Failed_Result()
    {
        var uow = new Mock<IUnitOfWork>();
        var behavior = new UnitOfWorkBehavior<TransactionalCommand, Result<string>>(uow.Object);
        var context = new RequestContext<TransactionalCommand, Result<string>>(new TransactionalCommand("a"), "user");

        await behavior.Handle(context, () => Task.FromResult(Result.Failure<string>(ResultError.Failure("X", "nope"))));

        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Does_Not_Commit_For_NonTransactional_Request()
    {
        var uow = new Mock<IUnitOfWork>();
        var behavior = new UnitOfWorkBehavior<PlainCommand, Result<string>>(uow.Object);
        var context = new RequestContext<PlainCommand, Result<string>>(new PlainCommand("a"), "user");

        await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")));

        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void AddMediarqUnitOfWork_Registers_UnitOfWork_And_Behavior()
    {
        var services = new ServiceCollection();

        services.AddMediarqUnitOfWork<FakeUnitOfWork>();

        services.Should().Contain(d => d.ServiceType == typeof(IUnitOfWork) && d.ImplementationType == typeof(FakeUnitOfWork));
        services.Should().Contain(d => d.ServiceType == typeof(IPipelineBehavior<,>)
            && d.ImplementationType == typeof(UnitOfWorkBehavior<,>));
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
    }
}
