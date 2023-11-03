using FluentMigrator;

namespace Ok.Movies.Database.Migrations._2023;

[Migration(202310060525)]
public class CreateRatingsTable : Migration
{
    private const string RatingsTableName = "ratings";
    private const string MoviesTableName = "movies";
    private const string UserIdColumName = "user_id";
    private const string MovieIdColumName = "movie_id";
    private const string IdColumName = "id";
    private const string RatingColumName = "rating";

    public override void Up()
    {
        Create.Table(RatingsTableName)
            .WithColumn(UserIdColumName).AsGuid()
            .WithColumn(MovieIdColumName).AsGuid().ForeignKey(MoviesTableName, IdColumName).OnDeleteOrUpdate(System.Data.Rule.Cascade)
            .WithColumn(RatingColumName).AsInt32().NotNullable();

        Create.PrimaryKey("PK_ratings_Id")
            .OnTable(RatingsTableName)
            .Columns(UserIdColumName, MovieIdColumName);
    }

    public override void Down()
    {
        Delete.Table(RatingsTableName);
    }
}
