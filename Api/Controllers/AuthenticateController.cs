using System.Security.Claims;
using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces;
using Application.Services;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Options;
using Infrastructure.Models;
using Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication1.Authorization.RequirementsData;
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
    private readonly ISessions _sessionsService;
    private readonly IUserCash _userCash;
    
    public AuthenticateController(AuthService authService, IOptions<JwtOptions> jwtOptions, ISessions sessionsService, IUserCash userCash)
    {
        _authService = authService;
        _accessTokenPath = jwtOptions.Value.AccessTokenStorage;
        _refreshTokenPath = jwtOptions.Value.RefreshTokenStorage;
        _sessionsService = sessionsService;
        _userCash = userCash;
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
        Console.WriteLine($"Session: {sessionId}");
        
        if (string.IsNullOrEmpty(sessionId))
            throw new InvalidTokenException();
        
        var tokens = await _authService.Refresh(sessionId);
        
        AppendCookies(tokens);
        return Ok();
    }

    // [AdminLevelAuth((EUserPermissions) 1)]
    [HttpOptions("admin")]
    // [Route("admin")]
    
    public async Task<IActionResult> GetAdminOptions()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var sessionId = User.FindFirst(ClaimTypes.SerialNumber)?.Value;

        if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(sessionId) || !int.TryParse(userIdStr, out var userId)) 
            throw new InvalidTokenException();

        var session = _sessionsService.GetSession(sessionId);
        var user = _userCash.GetUser(userId);
        await Task.WhenAll(session, user);
        
        if (session == null)
            throw new InvalidTokenException();
        
        return Ok(user.Result.AccessLevel.PermittedPanels());
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