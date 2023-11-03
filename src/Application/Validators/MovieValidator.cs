using FluentValidation;
using Ok.Movies.Application.Models;
using Ok.Movies.Application.Repositories;

namespace Ok.Movies.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;

    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
        
        RuleFor(movie => movie.Id).NotEmpty();
        RuleFor(movie => movie.Genres).NotEmpty();
        RuleFor(movie => movie.Title).NotEmpty();
        RuleFor(movie => movie.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(movie => movie.Slug).MustAsync(BeASlugForUniqueMovie)
            .Unless(movie => string.IsNullOrEmpty(movie.Title))
            .WithMessage("This movie already exists in the system");
    }

    private async Task<bool> BeASlugForUniqueMovie(Movie movie, string slug, CancellationToken token = default)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug);
        
        var isMovieUnique = existingMovie is null || existingMovie.Id == movie.Id;  
        
        return isMovieUnique;
    }
}
