using Bogus;
using Ok.Movies.MinimalAPI.Contracts.Requests;

namespace Ok.Movies.Tests.Integration.Core;

public sealed class CreateMovieRequestFaker:Faker<CreateMovieRequest>
{
    public CreateMovieRequestFaker()
    {
        RuleFor(request => request.Title, faker => faker.Lorem.Sentence());
        RuleFor(request => request.Genres, faker => faker.Lorem.Words().ToList());
        RuleFor(request => request.YearOfRelease, faker => faker.Date.Past(10).Year);
    }
}
