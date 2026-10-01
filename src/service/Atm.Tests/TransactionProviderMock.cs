using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Moq;
using System.Data;

namespace Atm.Tests;

internal static class TransactionProviderMock
{
    public static IPersistenceTransactionProvider Create()
    {
        var mock = new Mock<IPersistenceTransactionProvider>();

        mock.Setup(p => p.BeginTransactionAsync(It.IsAny<IsolationLevel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FakeTransaction());

        return mock.Object;
    }

    private sealed class FakeTransaction : IPersistenceTransaction
    {
        public ValueTask CommitAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask RollbackAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
