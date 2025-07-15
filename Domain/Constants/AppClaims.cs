namespace Domain.Constants;

public static class AppClaims
{
    // 'const' is best for true compile-time constants like this.
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
}