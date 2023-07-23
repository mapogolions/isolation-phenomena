using System.Data.Common;
using IsoLevelsAdoNet.Models;

namespace IsoLevelsAdoNet.Repos;

public interface IAlbumRepository : IBaseRepository
{
    Task<Album?> GetAsync(int id, CancellationToken cancellationToken);
    Task<Album?> GetAsync(int id, DbTransaction transaction, CancellationToken cancellationToken);
    Task<IEnumerable<Album>> GetAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Album>> GetAsync(DbTransaction transaction, CancellationToken cancellationToken);
    Task<int> AddAsync(Album album, CancellationToken cancellationToken);
    Task<int> AddAsync(Album album, DbTransaction transaction, CancellationToken cancellationToken);
    Task<int> UpdateAsync(Album album, CancellationToken cancellationToken);
    Task<int> UpdateAsync(Album album, DbTransaction transaction, CancellationToken cancellationToken);
    Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<int> DeleteAsync(int id, DbTransaction transaction, CancellationToken cancellationToken);
    Task<decimal> TotalCostAsync(CancellationToken cancellationToken);
    Task<decimal> TotalCostAsync(DbTransaction transaction, CancellationToken cancellationToken);
}
