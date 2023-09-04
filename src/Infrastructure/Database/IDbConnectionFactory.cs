using System.Data;

namespace Infrastructure.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}
