namespace Application.Interfaces;

public interface ITokenGenerator
{
    public (string accessToken, string refreshToken) GenerateTokens(ICollection<string> claims);
}