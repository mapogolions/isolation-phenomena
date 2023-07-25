using System.Data;
using System.Data.Common;

namespace IsolationPhenomena.Repos;

public interface IBaseRepository
{
    Task<T> TransactionScopeAsync<T>(
        Func<DbTransaction, CancellationToken, Task<T>> block,
        IsolationLevel iso,
        CancellationToken cancellationToken);

     Task TransactionScopeAsync(
        Func<DbTransaction, CancellationToken, Task> block,
        IsolationLevel iso,
        CancellationToken cancellationToken);
}
