using Bogus;
using Contracts.Requests;

namespace Ok.Movies.Tests.Integration.Api.Controllers;

public static class MovieFaker
{
    public static Faker<CreateMovieRequest> CreateMovieRequestGenerator() =>
        new Faker<CreateMovieRequest>()
            .RuleFor(request => request.Title, faker => faker.Lorem.Sentence())
            .RuleFor(request => request.Genres, faker => faker.Lorem.Words())
            .RuleFor(request => request.YearOfRelease, faker => faker.Date.Soon().Year);
}
