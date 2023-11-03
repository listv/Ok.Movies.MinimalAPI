using System.Data;

namespace Ok.Movies.Infrastructure.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}
