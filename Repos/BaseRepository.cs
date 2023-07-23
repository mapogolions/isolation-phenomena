using System.Data;
using System.Data.Common;

namespace IsolationPhenomena.Repos;


public class BaseRepository : IBaseRepository
{
    public BaseRepository(DbConnectionFactoryDelegate connectionFactory)
    {
        ConnectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    protected DbConnectionFactoryDelegate ConnectionFactory { get; }

    public async Task<T> TransactionScope<T>(
        Func<DbTransaction, CancellationToken, Task<T>> block,
        IsolationLevel iso,
        CancellationToken cancellationToken)
    {
        using (var connection = ConnectionFactory())
        {
            await connection.OpenAsync(cancellationToken);
            using (var transaction = connection.BeginTransaction(iso))
            {
                return await block(transaction, cancellationToken);
            }
        }
    }

    public async Task TransactionScope(
        Func<DbTransaction, CancellationToken, Task> block,
        IsolationLevel iso,
        CancellationToken cancellationToken)
    {
        using (var connection = ConnectionFactory())
        {
            await connection.OpenAsync(cancellationToken);
            using (var transaction = connection.BeginTransaction(iso))
            {
                await block(transaction, cancellationToken);
            }
        }
    }

    protected Func<DbTransaction, CancellationToken, Task<T>> AutoCommit<T>(Func<DbTransaction, CancellationToken, Task<T>> fn)
    {
        ArgumentNullException.ThrowIfNull(fn);
        return async (transaction, cancellation) =>
        {
            var res = await fn(transaction, cancellation);
            await transaction.CommitAsync(cancellation);
            return res;
        };
    }

    protected Func<DbTransaction, CancellationToken, Task> AutoCommit<T>(Func<DbTransaction, CancellationToken, Task> fn)
    {
        ArgumentNullException.ThrowIfNull(fn);
        return async (transaction, cancellation) =>
        {
            await fn(transaction, cancellation);
            await transaction.CommitAsync(cancellation);
        };
    }
}
