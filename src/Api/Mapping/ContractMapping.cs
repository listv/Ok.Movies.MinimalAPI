﻿using Application.Models;
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

    public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies)
    {
        return new MoviesResponse
        {
            Items = movies.Select(MapToResponse)
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

    public static MoviesFilteringOptions MapToOptions(this MoviesFilteringRequest request)
    {
        return new MoviesFilteringOptions
        {
            YearOfRelease = request.Year,
            Title = request.Title
        };
    }

    public static MoviesFilteringOptions WithUser(this MoviesFilteringOptions options, Guid? userId)
    {
        options.UserId = userId;
        return options;
    }
}
