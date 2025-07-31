using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Options;
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService: ITokenGenerator
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters tokenValidationParameters;
    public JwtOptions JwtOptions { get; }
    
    public TokenService(IOptions<JwtOptions> jwtOptions)
    {

        this.JwtOptions = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        
        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.JwtOptions.Secret)),
            SecurityAlgorithms.HmacSha256);
        
        tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingCredentials.Key,
            // ValidIssuer = _jwtSettings.Issuer,
            // ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true 
        };
    }



    public AuthKeyPairDto GenerateTokens(int userId, string sessionId)
    {
        return new AuthKeyPairDto
        {
            AccessToken = GenerateAccToken(userId, sessionId, JwtOptions.AccessTokenExpiration),
            RefreshToken = GenerateRefToken(sessionId, JwtOptions.RefreshTokenExpiration)
        };
    }

    private string GenerateAccToken(int userId, string sessionId, int expirationMinutes)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.SerialNumber, sessionId)
                
            ]),
            // Issuer = _jwtSettings.Issuer,
            // Audience = _jwtSettings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = _signingCredentials
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }
    
    private string GenerateRefToken(string sessionId, int expirationMinutes)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.SerialNumber, sessionId),
                
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