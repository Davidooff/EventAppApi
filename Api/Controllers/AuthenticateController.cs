using System.Security.Claims;
using Application.Exceptions;
using Application.Services;
using Domain.Constants;
using Domain.Entities;
using Domain.Options;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication1.Dto;
using WebApplication1.Filters;

[ApiController]
[Route("auth")]
[ServiceFilter(typeof(UsersExceptionFilter))]
public class AuthenticateController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly string _accessTokenPath;
    private readonly string _refreshTokenPath;

    
    public AuthenticateController(AuthService authService, IOptions<JwtOptions> jwtOptions)
    {
        _authService = authService;
        _accessTokenPath = jwtOptions.Value.AccessTokenStorage;
        _refreshTokenPath = jwtOptions.Value.RefreshTokenStorage;
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginDto loginDtoDto)
    {
        var tokens = await _authService.LogIn(loginDtoDto);
        AppendCookies(tokens);
        return Ok();
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> SignUp(RegisterDto registerDto)
    {
        var tokens = await _authService.SignUp(registerDto);
        AppendCookies(tokens);
        return Ok();
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var sessionId = User.FindFirst(ClaimTypes.SerialNumber)?.Value;
        var keyGuid = User.FindFirst(ClaimTypes.Authentication)?.Value;
            
        if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(keyGuid))
            throw new InvalidTokenException();
        
        var tokens = await _authService.Refresh(
            int.Parse(sessionId), 
            int.Parse(keyGuid));
        
        AppendCookies(tokens);
        return Ok();
    }

    private void AppendCookies(AuthKeyPairDto authResultDto)
    {
        CookieOptions accOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
        };

        CookieOptions refOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth/refresh"
        };
    
        Response.Cookies.Append(_accessTokenPath, authResultDto.AccessToken, accOptions);
        Response.Cookies.Append(_refreshTokenPath, authResultDto.RefreshToken, refOptions);
    }
}