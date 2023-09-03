using Application.Database;
using Application.Models;
using Dapper;

namespace Application.Repositories;

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
                                                                         """, movie, cancellationToken: token)).ConfigureAwait(false);
        var isMovieUpdated = result > 0;
        if (isMovieUpdated)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                delete from genres where movie_id = @Id
                                                                """, new { movie.Id }, cancellationToken: token)).ConfigureAwait(false);

            var genres = movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre });
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into genres(movie_id, name)
                                                                values (@MovieId, @Name)
                                                                """, genres, cancellationToken: token)).ConfigureAwait(false);
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
                                                                         """, movie, cancellationToken: token)).ConfigureAwait(false);
        var isMovieInserted = result > 0;

        if (isMovieInserted)
        {
            var genres = movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre });
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into genres (movie_id, name)
                                                                values(@MovieId, @Name)
                                                                """, genres, cancellationToken: token)).ConfigureAwait(false);
        }

        transaction.Commit();

        return isMovieInserted;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.id, m.slug, m.title, m.year_of_release YearOfRelease from movies m
            where id = @id
            """, new { id }, cancellationToken: token)).ConfigureAwait(false);

        if (movie is null) return movie;

        var genres = await connection.QueryAsync<string>(new CommandDefinition(
            """
            select name from genres
            where movie_id = @id
            """, new { id }, cancellationToken: token)).ConfigureAwait(false);

        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.id, m.slug, m.title, m.year_of_release YearOfRelease from movies m
            where slug = @slug
            """, new { slug }, cancellationToken: token)).ConfigureAwait(false);

        if (movie is null) return movie;

        var genres = await connection.QueryAsync<string>(new CommandDefinition(
            """
            select name from genres
            where movie_id = @Id
            """, new { movie.Id }, cancellationToken: token)).ConfigureAwait(false);

        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        var result = await connection.QueryAsync(new CommandDefinition("""
                                                                       select m.*, string_agg(g.name, ',') as genres
                                                                       from movies m
                                                                       left join genres g on m.id = g.movie_id
                                                                       group by m.id
                                                                       """, cancellationToken: token)).ConfigureAwait(false);
        return result.Select(movie => new Movie
        {
            Id = movie.id,
            Title = movie.title,
            YearOfRelease = movie.year_of_release,
            Genres = Enumerable.ToList(movie.genres.Split(','))
        });
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genres where movie_id = @id
                                                            """, new { id }, cancellationToken: token)).ConfigureAwait(false);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from movies where id = @id
                                                                         """, new { id }, cancellationToken: token)).ConfigureAwait(false);

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token).ConfigureAwait(false);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               select count(1) from movies where id = @id
                                                                               """, new { id }, cancellationToken: token)).ConfigureAwait(false);
    }
}
