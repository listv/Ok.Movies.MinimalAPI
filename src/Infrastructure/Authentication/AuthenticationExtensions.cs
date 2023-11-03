using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ok.Movies.Infrastructure.Validation;

namespace Ok.Movies.Infrastructure.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<JwtOptions>, JwtOptionsValidator>();
        services
            .AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateFluently()
            .ValidateOnStart();
        
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var jwtOptions = scope.ServiceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
            x.TokenValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Key)),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                
            };
        });
        return services;
    }

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<AuthOptions>, AuthOptionsValidator>();
        services
            .AddOptions<AuthOptions>()
            .BindConfiguration(AuthOptions.SectionName)
            .ValidateFluently()
            .ValidateOnStart();
        
        return services.AddAuthorization(options =>
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var authOptions = scope.ServiceProvider.GetRequiredService<IOptions<AuthOptions>>().Value;
            
            options.AddPolicy(AuthConstants.AdminUserPolicyName,
                policy => policy.AddRequirements(new AdminAuthRequirement(authOptions.ApiKey)));

            options.AddPolicy(AuthConstants.TrustedMemberPolicyName,
                policy => policy.RequireAssertion(c =>
                    c.User.HasClaim(claim => claim is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
                    c.User.HasClaim(claim => claim is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })));
        });
    }
}
