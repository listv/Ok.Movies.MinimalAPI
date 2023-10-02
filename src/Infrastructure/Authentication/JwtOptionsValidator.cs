using FluentValidation;

namespace Infrastructure.Authentication;

public class JwtOptionsValidator:AbstractValidator<JwtOptions>
{
    public JwtOptionsValidator()
    {
        RuleFor(options => options.Audience)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(options => options.Key)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(options => options.Issuer)
            .NotEmpty()
            .MaximumLength(255);
    }
}
