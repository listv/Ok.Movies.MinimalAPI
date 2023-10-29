using Application.Models;
using Contracts.Requests;
using Contracts.Responses;

namespace Api.Mapping;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movie
        {
            Genres = (List<string>)request.Genres,
            Title = request.Title,
            Id = Guid.NewGuid(),
            YearOfRelease = request.YearOfRelease
        };
    }

    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie
        {
            Id = id,
            Genres = (List<string>)request.Genres,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease
        };
    }

    public static MovieResponse MapToResponse(this Movie movie)
    {
        return new MovieResponse
        {
            Genres = movie.Genres,
            Id = movie.Id,
            Title = movie.Title,
            Rating = movie.Rating,
            UserRating = movie.UserRating,
            Slug = movie.Slug,
            YearOfRelease = movie.YearOfRelease
        };
    }

    public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies, int page, int pageSize, int totalCount)
    {
        return new MoviesResponse
        {
            Items = movies.Select(MapToResponse),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
    {
        return ratings.Select(rating => new MovieRatingResponse
        {
            MovieId = rating.MovieId,
            Rating = rating.Rating,
            Slug = rating.Slug
        });
    }

    public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest request)
    {
        return new GetAllMoviesOptions
        {
            YearOfRelease = request.Year,
            Title = request.Title,
            SortField = request.SortBy?.Trim('+', '-').Replace('-', '_'),
            SortOrder = request.SortBy is null ? SortOrder.Unsorted :
                GetSortOrder(request.SortBy),
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static SortOrder GetSortOrder(string sortBy) => sortBy.StartsWith('-') ? SortOrder.Descending : SortOrder.Ascending;

    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId)
    {
        options.UserId = userId;
        return options;
    }
}
