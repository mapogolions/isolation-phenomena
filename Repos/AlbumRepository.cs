using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using IsolationPhenomena.Models;

namespace IsolationPhenomena.Repos;


public class AlbumRepository : BaseRepository, IAlbumRepository
{
    public AlbumRepository(
        DbConnectionFactoryDelegate connectionFactory) : base(connectionFactory)
    {
    }

    public Task<Album?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return this.TransactionScopeAsync(
            (transaction, cancellation) => GetAsync(id, transaction, cancellation),
            IsolationLevel.ReadCommitted,
            cancellationToken);
    }

    public async Task<Album?> GetAsync(int id, DbTransaction transaction, CancellationToken cancellationToken)
    {
        using (var command = transaction.Connection!.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "SELECT * FROM Album Where Id = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            Album? album = null;
            if (reader.HasRows)
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    album = new Album
                    {
                        Id = reader.GetFieldValue<int>(0),
                        Title = reader.GetFieldValue<string>(1),
                        Artist = reader.GetFieldValue<string>(2),
                        Price = reader.GetFieldValue<decimal>(3)
                    };
                }
            }
            return album;
        }
    }

    public Task<IEnumerable<Album>> GetAsync(CancellationToken cancellationToken)
    {
        return this.TransactionScopeAsync(GetAsync, IsolationLevel.ReadCommitted, cancellationToken);
    }

    public async Task<IEnumerable<Album>> GetAsync(DbTransaction transaction, CancellationToken cancellationToken)
    {
        using (var command = transaction.Connection!.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "SELECT * FROM Album";
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                var albums = new List<Album>();
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        albums.Add(new Album
                        {
                            Id = reader.GetFieldValue<int>(0),
                            Title = reader.GetFieldValue<string>(1),
                            Artist = reader.GetFieldValue<string>(2),
                            Price = reader.GetFieldValue<decimal>(3)
                        });
                    }
                }
                return albums;
            }
        }
    }

    public Task<int> AddAsync(Album album, CancellationToken cancellationToken)
    {
        return this.TransactionScopeAsync(
            AutoCommit((transaction, cancellation) => AddAsync(album, transaction, cancellation)),
            IsolationLevel.ReadCommitted,
            cancellationToken);
    }

    public async Task<int> AddAsync(Album album, DbTransaction transaction, CancellationToken cancellationToken)
    {
        using (var command = transaction.Connection!.CreateCommand())
        {
            command.CommandText = "INSERT INTO Album (Title, Artist, Price) VALUES (@Title, @Artist, @Price)";
            command.Transaction = transaction;
            command.Parameters.Add(new SqlParameter("@Title", album.Title));
            command.Parameters.Add(new SqlParameter("@Artist", album.Artist));
            command.Parameters.Add(new SqlParameter("@Price", album.Price));
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public Task<int> UpdateAsync(Album album, CancellationToken cancellationToken)
    {
        return this.TransactionScopeAsync(
            AutoCommit((transaction, cancellation) => UpdateAsync(album, transaction, cancellation)),
            IsolationLevel.ReadCommitted,
            cancellationToken);
    }

    public Task<int> UpdateAsync(Album album, DbTransaction transaction, CancellationToken cancellationToken)
    {
        using (var command = transaction.Connection!.CreateCommand())
        {
            command.CommandText = "UPDATE Album SET Title = @Title, Artist = @Artist, Price = @Price WHERE Id = @Id";
            command.Transaction = transaction;
            command.Parameters.Add(new SqlParameter("@Id", album.Id));
            command.Parameters.Add(new SqlParameter("@Title", album.Title));
            command.Parameters.Add(new SqlParameter("@Artist", album.Artist));
            command.Parameters.Add(new SqlParameter("@Price", album.Price));
            return command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return this.TransactionScopeAsync(
            AutoCommit((transaction, cancellation) => DeleteAsync(id, transaction, cancellation)),
            IsolationLevel.ReadCommitted,
            cancellationToken);
    }

    public Task<int> DeleteAsync(int id, DbTransaction transaction, CancellationToken cancellationToken)
    {
        using (var command = transaction.Connection!.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "DELETE FROM Album WHERE Id = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            return command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public Task<decimal> TotalCostAsync(CancellationToken cancellationToken)
    {
        return this.TransactionScopeAsync(
            (transaction, cancellation) => TotalCostAsync(transaction, cancellation),
            IsolationLevel.ReadCommitted,
            cancellationToken);
    }

    public async Task<decimal> TotalCostAsync(DbTransaction transaction, CancellationToken cancellationToken)
    {
        using (var command = transaction.Connection!.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "SELECT SUM(price) FROM Album";
            object? result = await command.ExecuteScalarAsync(cancellationToken);
            return result is decimal total  ? total : 0m;
        }
    }
}
