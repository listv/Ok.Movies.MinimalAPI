using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Api;
using Bogus;
using Contracts.Requests;
using Contracts.Responses;
using FluentAssertions;
using Infrastructure.Authentication;
using Ok.Movies.Tests.Integration.Core;
using Xunit;

namespace Ok.Movies.Tests.Integration.Api.Controllers.Movies;

[TestCaseOrderer("Ok.Movies.Tests.Integration.Core.PriorityOrderer", "Ok.Movies.Tests.Integration")]
public class GetAllMoviesControllerTests : IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _apiFactory;

    public GetAllMoviesControllerTests(TestApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }

    [Fact]
    [TestPriority(0)]
    public async Task GetAll_ShouldReturnEmptyItems_WhenNoMoviesExist()
    {
        var response = await GetNonAuthenticatedClient()
            .GetAsync(ApiEndpoints.Movies.GetAll);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().BeEmpty();
    }

    [Fact]
    [TestPriority(1)]
    public async Task GetAll_ShouldReturnAllMovies_WhenMoviesExist()
    {
        // Arrange
        var movie = new CreateMovieRequestFaker().Generate();
        var authorizedClient = GetAuthenticatedClientWithTrustedMemberClaim();

        var createdResponse = await authorizedClient.PostAsJsonAsync(ApiEndpoints.Movies.Create, movie);
        var createdMovie = await createdResponse.Content.ReadFromJsonAsync<MovieResponse>();

        // Act
        var response = await authorizedClient.GetAsync(ApiEndpoints.Movies.GetAll);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var moviesResponse = await response.Content.ReadFromJsonAsync<MoviesResponse>();
        moviesResponse!.Items.Should().HaveCount(1);
        moviesResponse!.Items.Should().Contain(movieResponse => movieResponse.Id == createdMovie.Id);

        // Cleanup resources
        await DeleteMoviesAsync(createdMovie!.Id);
    }

    [Fact]
    [TestPriority(2)]
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
    [TestPriority(3)]
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
    [TestPriority(4)]
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
    [TestPriority(5)]
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
    [TestPriority(6)]
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
    [TestPriority(7)]
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

    [Fact(Skip = "Problems with difference in sorting on db side and in code")]
    [TestPriority(8)]
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

    [Fact(Skip = "Problems with difference in sorting on db side and in code")]
    [TestPriority(9)]
    public async Task GetAll_ShouldReturnSortedByTitleDesc_WhenMovieExists()
    {
        // Arrange
        var movies = new CreateMovieRequestFaker().Generate(10);
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
    [TestPriority(10)]
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

        // Act
        var response = await GetNonAuthenticatedClient()
            .GetAsync($"{ApiEndpoints.Movies.GetAll}?sortby={filter}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<List<Guid>> CreateMoviesAsync(List<CreateMovieRequest> movies)
    {
        var movieIds = new List<Guid>();
        var authorizedClient = GetAuthenticatedClientWithTrustedMemberClaim();

        foreach (var movie in movies)
        {
            var createdResponse = await authorizedClient
                .PostAsJsonAsync(ApiEndpoints.Movies.Create, movie)
                .ConfigureAwait(false);

            var createdMovie = await createdResponse.Content
                .ReadFromJsonAsync<MovieResponse>()
                .ConfigureAwait(false);

            movieIds.Add(createdMovie!.Id);
        }

        return movieIds;
    }

    private async Task DeleteMoviesAsync(params Guid[] ids)
    {
        var client = GetAuthenticatedClientWithAdminUserClaim();
        var tasks = ids.Select(id => client
                .DeleteAsync($"{ApiEndpoints.Movies.Base}/{id}"))
            .ToList();

        await Task.WhenAll(tasks);
    }

    private HttpClient GetAuthenticatedClientWithTrustedMemberClaim()
    {
        return _apiFactory.CreateAndConfigureClient(claims:
            new Claim(AuthConstants.TrustedMemberClaimName, "true"));
    }

    private HttpClient GetNonAuthenticatedClient()
    {
        return _apiFactory.CreateClient();
    }

    private HttpClient GetAuthenticatedClientWithAdminUserClaim()
    {
        return _apiFactory.CreateAndConfigureClient(claims: new Claim(AuthConstants.AdminUserClaimName, "true"));
    }
}