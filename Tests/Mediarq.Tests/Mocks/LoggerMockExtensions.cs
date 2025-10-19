using Microsoft.Extensions.Logging;
using Moq;

namespace Mediarq.Tests.Mocks;

public static class LoggerMockExtensions
{
    public static void VerifyLog<T>(
            this Mock<ILogger<T>> logger,
            LogLevel level,
            Times times,
            Func<string, bool> messagePredicate)
    {
        logger.Verify(x =>
            x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    messagePredicate(state.ToString()!)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            times);
    }
}
