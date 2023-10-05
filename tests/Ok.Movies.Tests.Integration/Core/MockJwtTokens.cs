using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Ok.Movies.Tests.Integration.Core;

public static class MockJwtTokens
{
    public static string Issuer { get; } = Guid.NewGuid().ToString();
    public static SecurityKey SecurityKey { get; }
    private static SigningCredentials SigningCredentials { get; }

    private static readonly JwtSecurityTokenHandler SecurityTokenHandler = new();
    private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();
    private static readonly byte[] PasswordInBytes = new byte[32];

    static MockJwtTokens()
    {
        RandomNumberGenerator.GetBytes(PasswordInBytes);
        SecurityKey = new SymmetricSecurityKey(PasswordInBytes);
        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);
    }

    public static string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TimeSpan.FromHours(8)),
            Issuer = Issuer,
            Audience = "https://movies.ok.com",
            SigningCredentials = SigningCredentials
        };
        
        var token = SecurityTokenHandler.CreateToken(tokenDescriptor);
        
        return SecurityTokenHandler.WriteToken(token);
    }
}
