using FluentValidation;

namespace Infrastructure.Cache;

public class RedisCacheOptionsValidator : AbstractValidator<RedisCacheOptions>
{
    public RedisCacheOptionsValidator()
    {
        RuleFor(o => o.ConnectionString)
            .NotEmpty();
    }
}
