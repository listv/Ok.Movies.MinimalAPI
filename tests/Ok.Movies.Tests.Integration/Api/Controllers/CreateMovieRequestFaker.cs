﻿using Bogus;
using Contracts.Requests;

namespace Ok.Movies.Tests.Integration.Api.Controllers;

public sealed class CreateMovieRequestFaker:Faker<CreateMovieRequest>
{
    public CreateMovieRequestFaker()
    {
        RuleFor(request => request.Title, faker => faker.Lorem.Sentence());
        RuleFor(request => request.Genres, faker => faker.Lorem.Words().ToList());
        RuleFor(request => request.YearOfRelease, faker => faker.Date.Soon().Year);
    }
}
