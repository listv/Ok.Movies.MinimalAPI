using Application.Database;
using Application.Models;
using Dapper;

namespace Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         update movies
                                                                         set slug = @Slug, title = @Title, year_of_release= @YearOfRelease
                                                                         where id = @Id
                                                                         """, movie));
        var isMovieUpdated = result > 0;
        if (isMovieUpdated)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                delete from genres where movie_id = @Id
                                                                """, new { movie.Id }));

            var genres = movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre });
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into genres(movie_id, name)
                                                                values (@MovieId, @Name)
                                                                """, genres));
        }

        transaction.Commit();

        return isMovieUpdated;
    }

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        var result = await connection.ExecuteAsync("""
                                                   insert into movies(id, slug, title, year_of_release)
                                                   values (@Id, @Slug, @Title, @YearOfRelease)
                                                   """, movie);
        var isMovieInserted = result > 0;

        if (isMovieInserted)
        {
            var genres = movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre });
            await connection.ExecuteAsync("""
                                          insert into genres (movie_id, name)
                                          values(@MovieId, @Name)
                                          """,
                genres);
        }

        transaction.Commit();

        return isMovieInserted;
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.id, m.slug, m.title, m.year_of_release YearOfRelease from movies m
            where id = @id
            """, new { id = id }));

        if (movie is null) return movie;

        var genres = await connection.QueryAsync<string>(new CommandDefinition(
            """
            select name from genres
            where movie_id = @id
            """, new { id }));

        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.id, m.slug, m.title, m.year_of_release YearOfRelease from movies m
            where slug = @slug
            """, new { slug }));

        if (movie is null) return movie;

        var genres = await connection.QueryAsync<string>(new CommandDefinition(
            """
            select name from genres
            where movie_id = @Id
            """, new { movie.Id }));

        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.QueryAsync(new CommandDefinition("""
                                                                       select m.*, string_agg(g.name, ',') as genres
                                                                       from movies m
                                                                       left join genres g on m.id = g.movie_id
                                                                       group by m.id
                                                                       """));
        return result.Select(movie => new Movie
        {
            Id = movie.id,
            Title = movie.title,
            YearOfRelease = movie.year_of_release,
            Genres = Enumerable.ToList(movie.genres.Split(','))
        });
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genres where movie_id = @id
                                                            """, new { id }));
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from movies where id = @id
                                                                         """, new { id }));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               select count(1) from movies where id = @id
                                                                               """, new { id }));
    }
}
