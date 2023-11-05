using Dapper;
using Ok.Movies.MinimalAPI.Application.Models;
using Ok.Movies.MinimalAPI.Infrastructure.Database;

namespace Ok.Movies.MinimalAPI.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         update movies
                                                                         set slug = @Slug, title = @Title, year_of_release= @YearOfRelease
                                                                         where id = @Id
                                                                         """, movie, cancellationToken: token))
            .ConfigureAwait(false);
        var isMovieUpdated = result > 0;
        if (isMovieUpdated)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                delete from genres where movie_id = @Id
                                                                """, new { movie.Id }, cancellationToken: token))
                .ConfigureAwait(false);

            var genres = movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre });
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into genres(movie_id, name)
                                                                values (@MovieId, @Name)
                                                                """, genres, cancellationToken: token))
                .ConfigureAwait(false);
        }

        transaction.Commit();

        return isMovieUpdated;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        using var transaction = connection.BeginTransaction();
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into movies(id, slug, title, year_of_release)
                                                                         values (@Id, @Slug, @Title, @YearOfRelease)
                                                                         """, movie, cancellationToken: token))
            .ConfigureAwait(false);
        var isMovieInserted = result > 0;

        if (isMovieInserted)
        {
            var genres = movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre });
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into genres (movie_id, name)
                                                                values(@MovieId, @Name)
                                                                """, genres, cancellationToken: token))
                .ConfigureAwait(false);
        }

        transaction.Commit();

        return isMovieInserted;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.id, m.slug, m.title, m.year_of_release YearOfRelease, round(avg(r.rating), 1) as rating, myr.rating as userrating
            from movies m
                left join ratings r on m.id = r.movie_id
                left join ratings myr on m.id = myr.movie_id and myr.user_id = @userId
            where id = @id
            group by id, userrating
            """, new { id, userId }, cancellationToken: token)).ConfigureAwait(false);

        if (movie is null) return movie;

        var genres = await connection.QueryAsync<string>(new CommandDefinition(
            """
            select name from genres
            where movie_id = @id
            """, new { id }, cancellationToken: token)).ConfigureAwait(false);

        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.id, m.slug, m.title, m.year_of_release YearOfRelease, round(avg(r.rating), 1) as rating, myr.rating as userrating
            from movies m
            left join ratings r on m.id = r.movie_id
            left join ratings myr on m.id = myr.movie_id and myr.user_id = @userId
            where slug = @slug
            group by id, userrating
            """, new { slug, userId }, cancellationToken: token)).ConfigureAwait(false);

        if (movie is null) return movie;

        var genres = await connection.QueryAsync<string>(new CommandDefinition(
            """
            select name from genres
            where movie_id = @Id
            """, new { movie.Id }, cancellationToken: token)).ConfigureAwait(false);

        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);

        var orderClause = string.Empty;
        if (options.SortField is not null)
        {
            orderClause = $"""
                           , m.{options.SortField}
                           order by m.{options.SortField} {(options.SortOrder == SortOrder.Ascending ? "asc" : "desc")}
                           """;
        }

        var result = await connection.QueryAsync(new CommandDefinition($"""
                                                                       select m.*, string_agg(g.name, ',') as genres,
                                                                              round(avg(r.rating), 1) as rating,
                                                                              myr.rating as userrating
                                                                       from movies m
                                                                       left join genres g on m.id = g.movie_id
                                                                       left join ratings r on m.id = r.movie_id
                                                                       left join ratings myr on m.id = myr.movie_id and myr.user_id = @userId
                                                                       where (@title is null or m.title like('%' || @title || '%'))
                                                                       and (@yearOfRelease is null or m.year_of_release = @yearOfRelease)
                                                                       group by m.id, myr.rating {orderClause}
                                                                       limit @pageSize
                                                                       offset @pageOffset
                                                                       """, options, cancellationToken: token))
            .ConfigureAwait(false);
        return result.Select(movie => new Movie
        {
            Id = movie.id,
            Title = movie.title,
            YearOfRelease = movie.year_of_release,
            Rating = (float?)movie.rating,
            UserRating = (int?)movie.userrating,
            Genres = Enumerable.ToList(movie.genres.Split(','))
        });
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token =
        default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genres where movie_id = @id
                                                            """, new { id }, cancellationToken: token))
            .ConfigureAwait(false);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from movies where id = @id
                                                                         """, new { id }, cancellationToken: token))
            .ConfigureAwait(false);

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               select count(1) from movies where id = @id
                                                                               """, new { id },
            cancellationToken: token)).ConfigureAwait(false);
    }

    public async Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        return await connection.QuerySingleAsync<int>(new CommandDefinition("""

                                                                            select count(id) from movies
                                                                            where (@title is null or title like('%' || @title || '%'))
                                                                            and (@yearOfRelease is null or year_of_release = @yearOfRelease)
                                                                            """,
                new { title, yearOfRelease }, cancellationToken: token))
            .ConfigureAwait(false);
    }
}
