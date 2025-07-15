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
    private readonly IdentityDbContext<User, IdentityRole<int>, int> _context;
    
    public AuthService(IIdentityService identityService, ITokenGenerator tokenGenerator, IdentityDbContext<User, IdentityRole<int>, int> context)
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
        _context.SaveChanges();
        var (accT, refT) = _tokenGenerator.GenerateTokens(user.Id, newSession.Id);
        return new AuthResultDto{accessToken = accT, refreshToken = accT};
    }
}