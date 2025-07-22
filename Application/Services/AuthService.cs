using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Models;
using WebApplication1.Dto;

namespace Application.Services;

public class AuthService
{
    private readonly IIdentityService _identityService;
    private readonly ISessionService _sessionService;
    
    public AuthService(IIdentityService identityService, ISessionService sessionService)
    {
        _identityService = identityService;
        _sessionService = sessionService;
    }

    public async Task<AuthKeyPairDto> LogIn(LoginDto loginDtoDto)
    {
        var user = await _identityService.GetUserByEmailAsync(loginDtoDto.Email);
        if (user == null)
            throw new UserNotFoundException();

        if (!await _identityService.CheckPasswordByUserAsync(user, loginDtoDto.Password))
            throw new InvalidCredentialsException();
        
        var tokens = await _sessionService.Create(user.Id);
        return tokens;
    }
    
    public async Task<AuthKeyPairDto> SignUp(RegisterDto registerDto)
    {
        var user = await _identityService.GetUserByEmailAsync(registerDto.Email);
        if (user != null)
        {
            throw new UserAlreadyExistsException();
        }

        var newUser = await _identityService.CreateAsync(registerDto);

        if (newUser == null)
            throw new UnableToCreateException();
        
        var tokens = await _sessionService.Create(user.Id);
        return tokens;
    }

    public async Task<AuthKeyPairDto> Refresh(int sessionId, int keyUidPayload)
    {
        var tokens = await _sessionService.Refresh(sessionId, keyUidPayload);
        return tokens;
    }
}