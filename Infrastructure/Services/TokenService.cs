using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Options;
using Infrastructure.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService: ITokenGenerator
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly SigningCredentials _signingCredentials;
    public JwtOptions JwtOptions { get; }
    
    public TokenService(IOptions<JwtOptions> jwtOptions)
    {

        this.JwtOptions = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.JwtOptions.Secret)),
            SecurityAlgorithms.HmacSha256);
    }



    public AuthKeyPairDto GenerateTokens(int userId, int sessionId, int uuid)
    {
        return new AuthKeyPairDto
        {
            AccessToken = GenerateAccToken(userId, sessionId, JwtOptions.AccessTokenExpiration),
            RefreshToken = GenerateRefToken(userId, sessionId, uuid, JwtOptions.RefreshTokenExpiration)
        };
    }

    private string GenerateAccToken(int userId, int sessionId, int expirationMinutes)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.SerialNumber, sessionId.ToString())
                
            ]),
            // Issuer = _jwtSettings.Issuer,
            // Audience = _jwtSettings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = _signingCredentials
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }
    
    private string GenerateRefToken(int userId, int sessionId, int uuid, int expirationMinutes)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.SerialNumber, sessionId.ToString()),
                new Claim(ClaimTypes.Authentication, uuid.ToString())
                
            ]),
            // Issuer = _jwtSettings.Issuer,
            // Audience = _jwtSettings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = _signingCredentials
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    public string GetId(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingCredentials.Key,
            ValidateIssuer = false,
            // ValidIssuer = _jwtSettings.Issuer,
            // ValidAudience = _jwtSettings.Audience,
            ValidateAudience = false,
            ValidateLifetime = true 
        };
        string id;
        try
        {
            id = _tokenHandler.ValidateToken(token, tokenValidationParameters, out _)
                .FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidTokenException();
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            throw new InvalidTokenException();
        }

        return id;
    }
}