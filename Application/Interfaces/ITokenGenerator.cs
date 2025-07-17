namespace Application.Interfaces;

public interface ITokenGenerator
{
    public (string accessToken, string refreshToken) GenerateTokens(int userId, int sessionId);
    public string GetId(string token);
    
}