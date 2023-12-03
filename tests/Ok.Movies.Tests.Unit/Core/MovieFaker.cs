using Application.Models;
using Bogus;

namespace Ok.Movies.Tests.Unit.Core;

public sealed class MovieFaker : Faker<Movie>
{
    public MovieFaker()
    {
        RuleFor(movie => movie.Id, faker => faker.Random.Guid());
        RuleFor(movie => movie.Genres, faker => faker.Lorem.Words().ToList());
        RuleFor(movie => movie.Title, faker => faker.Lorem.Word());
        RuleFor(movie => movie.YearOfRelease, faker => faker.Date.Past().Year);
    }
}
