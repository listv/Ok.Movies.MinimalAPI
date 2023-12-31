﻿using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Api;
using Bogus;
using Contracts.Responses;
using FluentAssertions;
using Infrastructure.Authentication;
using Ok.Movies.Tests.Integration.Core;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Movies;

public class GetMoviesControllerTests : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _authorizedClient;
    private readonly HttpClient _client;

    public GetMoviesControllerTests(TestApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _authorizedClient = apiFactory.CreateAndConfigureClient(
            claims: new Claim(AuthConstants.TrustedMemberClaimName, "true"));
    }

    [Fact]
    public async Task Get_ShouldReturnMovieById_WhenMovieExists()
    {
        // Arrange
        var createMovieRequest = new CreateMovieRequestFaker().Generate();
        var createdResponse = await _authorizedClient.PostAsJsonAsync(ApiEndpoints.Movies.Create, createMovieRequest);
        var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();

        // Act
        var response = await _client.GetAsync($"{ApiEndpoints.Movies.Base}/{createdMovie!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedMovie = await response.Content.ReadFromJsonAsync<MovieResponse>();
        retrievedMovie.Should().BeEquivalentTo(createdMovie);
    }

    [Fact]
    public async Task Get_ShouldReturnMovieBySlug_WhenMovieExists()
    {
        // Arrange
        var createMovieRequest = new CreateMovieRequestFaker().Generate();
        var createdResponse = await _authorizedClient.PostAsJsonAsync(ApiEndpoints.Movies.Create, createMovieRequest);
        var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();

        // Act
        var response = await _client.GetAsync($"{ApiEndpoints.Movies.Base}/{createdMovie!.Slug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedMovie = await response.Content.ReadFromJsonAsync<MovieResponse>();
        retrievedMovie.Should().BeEquivalentTo(createdMovie);
    }

    [Theory]
    [MemberData(nameof(GenerateInvalidIdOrSlug))]
    public async Task Get_ShouldReturnNotFound_WhenMovieDoesNotExist(string idOrSlug)
    {
        // Act
        var response = await _client.GetAsync($"{ApiEndpoints.Movies.Base}/{idOrSlug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public static IEnumerable<object[]> GenerateInvalidIdOrSlug()
    {
        yield return new object[] { Guid.NewGuid().ToString() };
        yield return new object[] { new Faker().Lorem.Slug() };
    }
}