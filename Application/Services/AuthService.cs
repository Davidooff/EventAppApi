using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Dto;

namespace Application.Services;

public class AuthService
{
    private readonly IIdentityService _identityService;
    
    public AuthService(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResultDto> LogIn(LoginDto loginDtoDto)
    {
        var user = await _identityService.GetUserByEmailAsync(loginDtoDto.Email);
        if (user == null)
        {
            throw new UserNotFoundException();
        }

        if (!await _identityService.CheckPasswordByUserAsync(user, loginDtoDto.Password))
        {
            throw new InvalidCredentialsException();
        }
        
        
    }
}