using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService: ITokenGenerator
{
    private readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
    private readonly JwtOptions _jwtSettings;
    private readonly SigningCredentials _signingCredentials;
    
    public TokenService(IOptions<JwtOptions> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);
    }
    
    public (string accessToken, string refreshToken) GenerateTokens(int userId, int sessionId)
    {
        return (GenerateToken(userId, _jwtSettings.accessTokenExpiration),
            GenerateToken(sessionId, _jwtSettings.refreshTokenExpiration));
    }
    
    public string GenerateToken(int userId, int expirationMinutes)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }),
            // Issuer = _jwtSettings.Issuer,
            // Audience = _jwtSettings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = _signingCredentials
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GetIdFromToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingCredentials.Key,
            ValidateIssuer = true,
            // ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            // ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true 
        };
        string id;
        try
        {
            id = tokenHandler.ValidateToken(token, tokenValidationParameters, out _)
                .FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidTokenException();
        } catch
        {
            throw new InvalidTokenException();
        }

        return id;
    }
}