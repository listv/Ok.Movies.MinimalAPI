using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace Ok.Movies.Infrastructure.Middleware;

public static class ProblemDetailsOptionsExtensions
{
    public static void MapFluentValidationException(this ProblemDetailsOptions options)
    {
        options.Map<ValidationException>((ctx, ex) =>
        {
            var factory = ctx.RequestServices.GetRequiredService<ProblemDetailsFactory>();

            var errors = ex.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(validationFailure => validationFailure.ErrorMessage).ToArray());

            return factory.CreateValidationProblemDetails(ctx, errors, StatusCodes.Status400BadRequest);
        });
    }
}
