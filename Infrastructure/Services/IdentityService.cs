using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Dto;

namespace Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;

    public IdentityService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Succeeded, int UserId)> CheckPasswordByEmailAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false, -1);
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

    public async Task<User?> CreateAsync(RegisterDto registerDtoDto)
    {
        Console.WriteLine(registerDtoDto);
        User user = new User
        {
            Email = registerDtoDto.Email,
            UserName = registerDtoDto.Username,
            FirstName = registerDtoDto.FirstName,
            LastName = registerDtoDto.LastName,
        }; 
        var res = await _userManager.CreateAsync(user, registerDtoDto.Password);

        if (res.Succeeded)
        {
            return user;
        } 
        
        foreach (var error in res.Errors)
        {
            Console.WriteLine($"- Code: {error.Code}, Description: {error.Description}");
        }
        
        return null;
    }
}