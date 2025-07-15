using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebApplication1.Dto;

namespace Application.Services;

public class AuthService
{
    private readonly IIdentityService _identityService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IDatabaseContext _context; 
    
    public AuthService(IIdentityService identityService, ITokenGenerator tokenGenerator, IDatabaseContext context)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
        _context = context;
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

        var newSession = new Sessions { UserId = user.Id };
        user.Sessions.Add(newSession);
        await _context.SaveChangesAsync();
        var (accT, refT) = _tokenGenerator.GenerateTokens(user.Id, newSession.Id);
        return new AuthResultDto{accessToken = accT, refreshToken = accT};
    }
    
    public async Task<AuthResultDto> SignUp(RegisterDto registerDto)
    {
        var user = await _identityService.GetUserByEmailAsync(registerDto.Email);
        if (user != null)
        {
            throw new UserAlreadyExistsException();
        }

       var newUser = await _identityService.CreateAsync(registerDto);

       if (newUser == null)
       {
           throw new UnableToCreateException();
       }
       else
       {
           var newSession = new Sessions { UserId = newUser.Id };
           _context.Sessions.Add(newSession);
           await _context.SaveChangesAsync();
           var (accT, refT) = _tokenGenerator.GenerateTokens(newUser.Id, newSession.Id);
           return new AuthResultDto{accessToken = accT, refreshToken = accT};
       } 
    }
}