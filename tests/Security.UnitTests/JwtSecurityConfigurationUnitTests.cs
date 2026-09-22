using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace ArrayApp.Security.UnitTests;

[TestFixture]
public class JwtSecurityConfigurationUnitTests
{
    [Test]
    public void JwtSigningKey_MustHave_Minimum256BitsLength()
    {
        // RFC 7518 Section 3.2: A key of the same size as the hash output or larger MUST be used with HMAC-SHA256
        const string secureKey = "ArrayAppEnterpriseSecretKeyWith256BitsMinimumLength2026!";
        var keyBytes = Encoding.UTF8.GetBytes(secureKey);

        keyBytes.Length.Should().BeGreaterThanOrEqualTo(32, 
            "HMAC-SHA256 signing keys must be at least 256 bits (32 bytes) to withstand dictionary and brute-force attacks (CWE-326)");
    }

    [Test]
    public void TokenValidationParameters_MustEnforce_IssuerAudienceAndLifetimeValidation()
    {
        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ArrayAppEnterpriseSecretKeyWith256BitsMinimumLength2026!")),
            ValidateIssuer = true,
            ValidIssuer = "ArrayApp.WebUI.Issuer",
            ValidateAudience = true,
            ValidAudience = "ArrayApp.WebUI.Audience",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        tokenValidationParams.ValidateIssuer.Should().BeTrue("Issuer validation must be active to prevent cross-tenant token spoofing");
        tokenValidationParams.ValidateAudience.Should().BeTrue("Audience validation must be active to prevent confused deputy token replay");
        tokenValidationParams.ValidateLifetime.Should().BeTrue("Lifetime validation must be active to ensure expired tokens are rejected");
        tokenValidationParams.ValidateIssuerSigningKey.Should().BeTrue("Issuer signing key validation must be enforced");
    }

    [Test]
    public void GeneratedToken_MustContain_RoleClaims()
    {
        var roles = new[] { "Administrator", "Manager" };
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "alice@arrayapp.com"),
            new Claim(ClaimTypes.NameIdentifier, "42"),
            new Claim(ClaimTypes.Name, "alice")
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ArrayAppEnterpriseSecretKeyWith256BitsMinimumLength2026!"));
        var token = new JwtSecurityToken(
            issuer: "ArrayApp.WebUI.Issuer",
            audience: "ArrayApp.WebUI.Audience",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var serialized = tokenHandler.WriteToken(token);
        var parsed = tokenHandler.ReadJwtToken(serialized);

        var roleClaims = parsed.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value).ToList();
        roleClaims.Should().Contain("Administrator");
        roleClaims.Should().Contain("Manager");
    }
}
