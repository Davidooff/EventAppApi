using Domain.Options;
using Infrastructure.Models;

namespace Application.Interfaces;

public interface ITokenGenerator
{
    public JwtOptions JwtOptions { get; }
    public AuthKeyPairDto GenerateTokens(int userId, string sessionId);
    public string GetId(string token);
    
}