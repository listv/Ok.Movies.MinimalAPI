using Application.Models;
using FluentValidation;

namespace Application.Validators;

public class MoviesFilteringOptionsValidator:AbstractValidator<MoviesFilteringOptions>
{
    public MoviesFilteringOptionsValidator()
    {
        RuleFor(movie => movie.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}
