namespace Application.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, string UserId)> CheckPasswordByEmailAsync(string email, string password);
    Task<bool> CheckPasswordByUserAsync(User user, string password);

    Task<User?> GetUserByEmailAsync(string email);
    // Add other methods you need, like CreateUser, GetRolesAsync, etc.
}