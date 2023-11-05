using Bogus;
using Ok.Movies.MinimalAPI.Contracts.Requests;

namespace Ok.Movies.Tests.Integration.Core;

public class RateMovieRequestFaker:Faker<RateMovieRequest>
{
    public RateMovieRequestFaker()
    {
        RuleFor(request => request.Rating, faker => faker.Random.Number(1, 5));
    }
}
