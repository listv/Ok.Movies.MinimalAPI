using Application.Models;
using FluentValidation;

namespace Application.Validators;

public class GetAllMoviesOptionsValidator:AbstractValidator<GetAllMoviesOptions>
{
    public static readonly string[] AcceptableSortFields = { "title", "year_of_release" };
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(movie => movie.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(options => options.SortField)
            .Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage("You can only sort by 'title' or 'year-of-release'");

        RuleFor(options => options.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(options => options.PageSize)
            .InclusiveBetween(1, 25)
            .WithMessage("You can get between 1 and 25 movies per page");
    }
}
