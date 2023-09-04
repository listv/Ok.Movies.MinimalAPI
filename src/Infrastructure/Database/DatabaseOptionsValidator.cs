using FluentValidation;
using Npgsql;

namespace Infrastructure.Database;

public class DatabaseOptionsValidator: AbstractValidator<DatabaseOptions>
{
    public DatabaseOptionsValidator()
    {
        RuleFor(options => options.ConnectionString)
            .NotEmpty()
            .Must(BeAValidConnectionString)
            .WithMessage("Invalid '{PropertyName}' value provided: '{PropertyValue}'.");

    }

    private bool BeAValidConnectionString(string connectionString)
    {
        var isValidConnectionString = true;
        try
        {
            _ = new NpgsqlConnectionStringBuilder(connectionString);
        }
        catch (Exception)
        {
            isValidConnectionString = false;
        }

        return isValidConnectionString;
    }
}
