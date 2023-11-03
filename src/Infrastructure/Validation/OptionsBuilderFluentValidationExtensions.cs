using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ok.Movies.Infrastructure.Validation;

public static class OptionsBuilderFluentValidationExtensions
{
    [RequiresUnreferencedCode(
        "Uses DataAnnotationValidateOptions which is unsafe given that the options type passed in when calling Validate cannot be statically analyzed so its" +
        " members may be trimmed.")]
    public static OptionsBuilder<TOptions> ValidateFluently<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(services =>
            new FluentValidateOptions<TOptions>(optionsBuilder.Name,
                services.GetRequiredService<IValidator<TOptions>>()));
        return optionsBuilder;
    }
}

public class FluentValidateOptions<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        TOptions>
    : IValidateOptions<TOptions> where TOptions : class
{
    private readonly IValidator<TOptions> _validator;

    [RequiresUnreferencedCode(
        "The implementation of Validate method on this type will walk through all properties of the passed in options object, and its type cannot be " +
        "statically analyzed so its members may be trimmed.")]
    public FluentValidateOptions(string? name, IValidator<TOptions> validator)
    {
        Name = name;
        _validator = validator;
    }

    public string? Name { get; }


    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode",
        Justification =
            "Suppressing the warnings on this method since the constructor of the type is annotated as RequiresUnreferencedCode.")]
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        if (Name != null && Name != name) return ValidateOptionsResult.Skip;

        if (options is null) throw new ArgumentNullException(nameof(options));

        var validationResult = _validator.Validate(options);

        if (validationResult.IsValid) return ValidateOptionsResult.Success;

        var errors = validationResult.Errors.Select(failure =>
            $"Options validation failed for '{failure.PropertyName}' with error: '{failure.ErrorMessage}'.");

        return ValidateOptionsResult.Fail(errors);
    }
}
