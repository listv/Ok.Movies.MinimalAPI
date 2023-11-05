using FluentMigrator;
using FluentMigrator.Postgres;

namespace Ok.Movies.MinimalAPI.Database.Migrations._2023;

[Migration(202308071510, TransactionBehavior.None)]
public class CreateSlugIndexOnMoviesTable:Migration
{
    private const string MoviesSlugIndexName = "movies_slug_idx";
    private const string MoviesTableName = "movies";
    private const string SlugColumnName = "slug";
    
    public override void Up()
    {
        Create.Index(MoviesSlugIndexName)
            .OnTable(MoviesTableName)
            .OnColumn(SlugColumnName)
            .Ascending()
            .WithOptions()
            .AsConcurrently()
            .UsingBTree()
            .Unique();

    }

    public override void Down()
    {
        Delete
            .Index(MoviesSlugIndexName)
            .OnTable(MoviesTableName);
    }
}
