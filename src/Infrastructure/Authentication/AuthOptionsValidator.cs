using FluentValidation;

namespace Ok.Movies.MinimalAPI.Infrastructure.Authentication;

public class AuthOptionsValidator : AbstractValidator<AuthOptions>
{
    public AuthOptionsValidator()
    {
        RuleFor(options => options.ApiKey)
            .NotEmpty()
            .Must(BeAValidGuid)
            .WithMessage(
                "Please ensure you have provided a valid {PropertyName}. '{PropertyValue}' is not a valid GUID.");
    }

    private static bool BeAValidGuid(string apiKey)
    {
        return Guid.TryParse(apiKey, out _);
    }
}
