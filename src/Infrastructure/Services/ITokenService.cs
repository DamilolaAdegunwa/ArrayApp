using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ArrayApp.Infrastructure.Services;
public interface ITokenSvc
{
    int TokenMaxMinute { get; }
    TokenDTO GenerateAccessTokenFromClaims(params Claim[] claims);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    TokenDTO GenerateAccessTokenFromClaimsV2(params Claim[] claims);
}

public class TokenService : ITokenSvc
{
    private readonly JwtConfig _jwtConfig;

    public TokenService(IOptions<JwtConfig> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value;
    }

    public int TokenMaxMinute => _jwtConfig.TokenDurationInSeconds;

    public TokenDTO GenerateAccessTokenFromClaims(params Claim[] claims)
    {
        var issued = DateTimeOffset.Now;
        var expires = DateTimeOffset.Now.AddSeconds(_jwtConfig.TokenDurationInSeconds);

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfig.SecurityKey));

        var jwtToken = new JwtSecurityToken(issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            notBefore: issued.LocalDateTime,
            expires: expires.LocalDateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new TokenDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = GenerateRefreshToken(),
            Expires = expires
        };
    }

    public TokenDTO GenerateAccessTokenFromClaimsV2(params Claim[] claims)
    {
        var expires = DateTimeOffset.Now.AddSeconds(_jwtConfig.TokenDurationInSeconds);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(_jwtConfig.SecurityKey);
        var tokenDescriptor = new SecurityTokenDescriptor 
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddSeconds(_jwtConfig.TokenDurationInSeconds),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenStr = tokenHandler.WriteToken(token);
        return new TokenDTO
        {
            Token = tokenStr,
            RefreshToken = GenerateRefreshToken(),
            Expires = expires
        };
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfig.SecurityKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
