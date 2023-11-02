using System.Text;
using FluentValidation;
using Infrastructure.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

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
        return services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthConstants.AdminUserPolicyName,
                policy => policy.RequireClaim(AuthConstants.AdminUserClaimName, "true"));

            options.AddPolicy(AuthConstants.TrustedMemberPolicyName,
                policy => policy.RequireAssertion(c =>
                    c.User.HasClaim(claim => claim is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
                    c.User.HasClaim(claim => claim is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })));
        });
    }

    public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services)
    {
        return services.AddScoped<ApiKeyAuthFilter>();
    }
}
