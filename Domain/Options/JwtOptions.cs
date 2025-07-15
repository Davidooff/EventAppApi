namespace Domain.Options;

public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    // public string Issuer { get; set; } = string.Empty;
    // public string Audience { get; set; } = string.Empty;

    public int AccessTokenExpiration { get; set; } = 0; //mins
    public int RefreshTokenExpiration { get; set; } = 0; //mins

    public string AccessTokenStorage { get; set; } = string.Empty;
    public string RefreshTokenStorage { get; set; } = string.Empty;
}
