using Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;

    public IdentityService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Succeeded, string UserId)> CheckPasswordByEmailAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false, null);
        }

        var success = await _userManager.CheckPasswordAsync(user, password);
        return (success, user.Id);
    }
    
    public async Task<bool> CheckPasswordByUserAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
}