using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Bogus;
using FluentAssertions;
using Ok.Movies.MinimalAPI.Api;
using Ok.Movies.MinimalAPI.Contracts.Requests;
using Ok.Movies.MinimalAPI.Contracts.Responses;
using Ok.Movies.MinimalAPI.Infrastructure.Authentication;
using Ok.Movies.Tests.Integration.Core;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Movies;

public class GetAllMoviesControllerTests : IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _apiFactory;

    public GetAllMoviesControllerTests(TestApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }

    [Fact]
    public async Task GetAll_ReturnsAllMovies_WhenMoviesExist()
    {
        // Arrange
        var movie = new CreateMovieRequestFaker().Generate();
        var authorizedClient = GetAuthenticatedClientWithTrustedMemberClaim();

        var createdResponse = await authorizedClient.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);
        var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();

        // Act
        var response = await GetNonAuthenticatedClient().GetAsync(ApiEndpoints.Movies.GetAll);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Single().Should().BeEquivalentTo(createdMovie);

        // Cleanup resources
        await DeleteMoviesAsync(createdMovie!.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyItems_WhenNoMoviesExist()
    {
        var response = await GetNonAuthenticatedClient()
            .GetAsync(ApiEndpoints.Movies.GetAll);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnBadRequest_WhenInvalidFilterProvided()
    {
        // Arrange
        var yearInFuture = DateTime.UtcNow.Year + 1;

        // Act
        var response = await GetNonAuthenticatedClient()
            .GetAsync($"{ApiEndpoints.Movies.GetAll}?year={yearInFuture}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_ShouldReturnFilteredByYear_WhenMovieExist()
    {
        // Arrange
        var movies = new CreateMovieRequestFaker().Generate(3);
        var movieIds = await CreateMoviesAsync(movies);

        // Act
        var response = await GetNonAuthenticatedClient()
            .GetAsync($"{ApiEndpoints.Movies.GetAll}?year={movies[0].YearOfRelease}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().OnlyContain(movie => movie.YearOfRelease == movies[0].YearOfRelease);

        // Cleanup
        await DeleteMoviesAsync(movieIds.ToArray());
    }

    [Fact]
    public async Task GetAll_ShouldReturnFilteredByTitle_WhenMovieExist()
    {
        // Arrange
        var movies = new CreateMovieRequestFaker().Generate(3);
        var movieIds = await CreateMoviesAsync(movies);

        var titleFilter = movies[0].Title.Split(' ')[1];

        // Act
        var response = await GetNonAuthenticatedClient()
            .GetAsync($"{ApiEndpoints.Movies.GetAll}?title={titleFilter}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().OnlyContain(movie => movie.Title.Contains(titleFilter));

        // Cleanup

        await DeleteMoviesAsync(movieIds.ToArray());
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenMovieDoesNotExistAndFilterBytTitleProvided()
    {
        // Arrange
        var titleFilter = new Faker().Lorem.Word();

        // Act
        var response = await GetNonAuthenticatedClient()
            .GetAsync($"{ApiEndpoints.Movies.GetAll}?title={titleFilter}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnSortedByYearAsc_WhenMovieExists()
    {
        // Arrange
        var movies = new CreateMovieRequestFaker().Generate(3);
        var movieIds = await CreateMoviesAsync(movies);

        // Act
        var response = await GetNonAuthenticatedClient()
            .GetAsync($"{ApiEndpoints.Movies.GetAll}?sortby=year-of-release");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().BeInAscendingOrder(m => m.YearOfRelease);

        // Cleanup
        await DeleteMoviesAsync(movieIds.ToArray());
    }

    [Fact]
    public async Task GetAll_ShouldReturnSortedByYearDesc_WhenMovieExists()
    {
        // Arrange
        var movies = new CreateMovieRequestFaker().Generate(3);
        var movieIds = await CreateMoviesAsync(movies);

        // Act
        var response = await GetNonAuthenticatedClient()
            .GetAsync($"{ApiEndpoints.Movies.GetAll}?sortby=-year-of-release");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().BeInDescendingOrder(m => m.YearOfRelease);

        // Cleanup
        await DeleteMoviesAsync(movieIds.ToArray());
    }

    [Fact]
    public async Task GetAll_ShouldReturnSortedByTitleAsc_WhenMovieExists()
    {
        // Arrange
        var movies = new CreateMovieRequestFaker().Generate(3);
        var movieIds = await CreateMoviesAsync(movies);

        // Act
        var response = await GetNonAuthenticatedClient()
            .GetAsync($"{ApiEndpoints.Movies.GetAll}?sortby=title");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().BeInAscendingOrder(m => m.Title);

        // Cleanup
        await DeleteMoviesAsync(movieIds.ToArray());
    }

    [Fact]
    public async Task GetAll_ShouldReturnSortedByTitleDesc_WhenMovieExists()
    {
        // Arrange
        var movies = new CreateMovieRequestFaker().Generate(3);
        var movieIds = await CreateMoviesAsync(movies);

        // Act
        var response = await GetNonAuthenticatedClient()
            .GetAsync($"{ApiEndpoints.Movies.GetAll}?sortby=-title");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().BeInDescendingOrder(m => m.Title);

        // Cleanup
        await DeleteMoviesAsync(movieIds.ToArray());
    }

    [Theory]
    [InlineData("id")]
    [InlineData("-id")]
    [InlineData("slug")]
    [InlineData("-slug")]
    [InlineData("rating")]
    [InlineData("-rating")]
    [InlineData("userrating")]
    [InlineData("-userrating")]
    public async Task GetAll_ShouldReturnBadRequest_WhenNotSupportedSortingFieldProvided(string filter)
    {
        // Arrange
        var movies = new CreateMovieRequestFaker().Generate(3);
        var movieIds = await CreateMoviesAsync(movies);

        // Act
        var response = await GetNonAuthenticatedClient()
            .GetAsync($"{ApiEndpoints.Movies.GetAll}?sortby={filter}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        await DeleteMoviesAsync(movieIds.ToArray());
    }
    
    private async Task<List<Guid>> CreateMoviesAsync(List<CreateMovieRequest> movies)
    {
        var authorizedClient = GetAuthenticatedClientWithTrustedMemberClaim();
        var movieIds = new List<Guid>();

        foreach (var movie in movies)
        {
            var createdResponse = await authorizedClient.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);
            var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();
            movieIds.Add(createdMovie!.Id);
        }

        return movieIds;
    }

    private async Task DeleteMoviesAsync(params Guid[] ids)
    {
        foreach (var id in ids)
        {
            await GetAuthenticatedClientWithAdminUserClaim()
                .DeleteAsync($"{ApiEndpoints.Movies.Base}/{id}");
        }
    }

    private HttpClient GetAuthenticatedClientWithTrustedMemberClaim() =>
        _apiFactory.CreateAndConfigureClient(
            new Claim(AuthConstants.TrustedMemberClaimName, "true"));

    private HttpClient GetNonAuthenticatedClient() =>
        _apiFactory.CreateClient();

    private HttpClient GetAuthenticatedClientWithAdminUserClaim() =>
        _apiFactory.CreateAndConfigureClient(new Claim(AuthConstants.AdminUserClaimName, "true"));
}
