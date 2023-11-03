using FluentMigrator;

namespace Ok.Movies.Database.Migrations._2023;

[Migration(202308071625)]
public class CreateGenresTable : Migration
{
    private const string GenresTableName = "genres";

    public override void Up()
    {
        Create.Table(GenresTableName)
            .WithColumn("movie_id").AsGuid().ForeignKey("movies", "id").OnDeleteOrUpdate(System.Data.Rule.Cascade)
            .WithColumn("name").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Table(GenresTableName);
    }
}
