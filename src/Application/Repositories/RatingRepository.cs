using Dapper;
using Ok.Movies.Application.Models;
using Ok.Movies.Infrastructure.Database;

namespace Ok.Movies.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into ratings(user_id, movie_id, rating)
                                                                         values (@userId, @movieId, @rating)
                                                                         on conflict(user_id, movie_id) do update
                                                                         set rating = @rating
                                                                         """, new { userId, movieId, rating },
            cancellationToken: token)).ConfigureAwait(false);

        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
                select round(avg(rating), 1)
                from ratings
                where movie_id = @movieId
                """, new { movieId }, cancellationToken: token))
            .ConfigureAwait(false);
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId,
        CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
                select round(avg(rating), 1),
                    (select rating
                    from ratings
                    where movie_id = @movieId
                    and user_id = @userId
                    limit 1)
                from ratings
                where movie_id = @movieId
                """, new { movieId, userId }, cancellationToken: token))
            .ConfigureAwait(false);
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from ratings
                                                                         where movie_id = @movieId
                                                                             and user_id = @userId
                                                                         """, new { movieId, userId },
                cancellationToken: token))
            .ConfigureAwait(false);

        return result > 0;
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QueryAsync<MovieRating>(new CommandDefinition("""
                                                                              select r.movie_id as movieId, m.slug, r.rating
                                                                              from ratings r
                                                                              inner join movies m on m.id = r.movie_id
                                                                              where r.user_id = @userId
                                                                              """, new { userId },
                cancellationToken: token))
            .ConfigureAwait(false);
    }
}
