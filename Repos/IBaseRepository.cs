using System.Data;
using System.Data.Common;

namespace IsoLevelsAdoNet.Repos;

public interface IBaseRepository
{
    Task<T> TransactionScope<T>(
        Func<DbTransaction, CancellationToken, Task<T>> block,
        IsolationLevel iso,
        CancellationToken cancellationToken);

     Task TransactionScope(
        Func<DbTransaction, CancellationToken, Task> block,
        IsolationLevel iso,
        CancellationToken cancellationToken);
}
