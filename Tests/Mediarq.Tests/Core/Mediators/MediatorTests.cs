namespace Mediarq.Tests.Core.Mediators
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Mediarq.Core.Common.Contexts;
    using Mediarq.Core.Common.Pipeline;
    using Mediarq.Core.Common.Requests.Abstraction;
    using Mediarq.Core.Mediators;
    using Moq;
    using Xunit;
    using TResponse = System.String;

    public class MediatorTests
    {
        private readonly Mediator _testClass;
        private ServiceFactory _serviceFactory;
        private readonly Mock<IRequestContextFactory> _requestContextFactory;
        private readonly Mock<IPipelineExecutor> _pipelineExecutor;

        public MediatorTests()
        {
            _serviceFactory = x => new object();
            _requestContextFactory = new Mock<IRequestContextFactory>();
            _pipelineExecutor = new Mock<IPipelineExecutor>();
            _testClass = new Mediator(_serviceFactory, _requestContextFactory.Object, _pipelineExecutor.Object);
        }

        [Fact]
        public void CanConstruct()
        {
            // Act
            var instance = new Mediator(_serviceFactory, _requestContextFactory.Object, _pipelineExecutor.Object);

            // Assert
            instance.Should().NotBeNull();
        }

        [Fact]
        public void CannotConstructWithNullServiceFactory()
        {
            FluentActions.Invoking(() => new Mediator(default(ServiceFactory), _requestContextFactory.Object, _pipelineExecutor.Object)).Should().Throw<ArgumentNullException>().WithParameterName("serviceFactory");
        }

        [Fact]
        public void CannotConstructWithNullRequestContextFactory()
        {
            FluentActions.Invoking(() => new Mediator(_serviceFactory, default(IRequestContextFactory), _pipelineExecutor.Object)).Should().Throw<ArgumentNullException>().WithParameterName("requestContextFactory");
        }

        [Fact]
        public void CannotConstructWithNullPipelineExecutor()
        {
            FluentActions.Invoking(() => new Mediator(_serviceFactory, _requestContextFactory.Object, default(IPipelineExecutor))).Should().Throw<ArgumentNullException>().WithParameterName("pipelineExecutor");
        }

        [Fact]
        public async Task CanCallSend()
        {
            // Arrange
            var request = new Mock<ICommandOrQuery<TResponse>>().Object;
            var cancellationToken = CancellationToken.None;

            _requestContextFactory.Setup(mock => mock.Create<ICommandOrQuery<TResponse>, TResponse>(It.IsAny<ICommandOrQuery<TResponse>>(), It.IsAny<CancellationToken>())).Returns(new object());

            // Act
            var result = await _testClass.Send<TResponse>(request, cancellationToken);

            // Assert
            _requestContextFactory.Verify(mock => mock.Create<ICommandOrQuery<TResponse>, TResponse>(It.IsAny<ICommandOrQuery<TResponse>>(), It.IsAny<CancellationToken>()));

            throw new NotImplementedException("Create or modify test");
        }

        [Fact]
        public async Task CannotCallSendWithNullRequest()
        {
            await FluentActions.Invoking(() => _testClass.Send<TResponse>(default(ICommandOrQuery<TResponse>), CancellationToken.None)).Should().ThrowAsync<ArgumentNullException>().WithParameterName("request");
        }
    }
}