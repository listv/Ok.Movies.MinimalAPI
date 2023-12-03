using FluentMigrator;

namespace Database.Migrations._2023;

[Migration(202307250751)]
public class CreateMoviesTable : Migration
{
    private const string MoviesTableName = "movies";
    private const string IdColumnName = "id";
    private const string SlugColumnName = "slug";
    private const string TitleColumnName = "title";
    private const string YearColumnName = "year_of_release";

    public override void Up()
    {
        Create.Table(MoviesTableName)
            .WithColumn(IdColumnName).AsGuid().PrimaryKey()
            .WithColumn(SlugColumnName).AsString().NotNullable()
            .WithColumn(TitleColumnName).AsString().NotNullable()
            .WithColumn(YearColumnName).AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table(MoviesTableName);
    }
}
