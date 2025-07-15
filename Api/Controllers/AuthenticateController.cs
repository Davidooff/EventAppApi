using Application.Services;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;

[ApiController]
[Route("auth")]
public class AuthenticateController : ControllerBase
{
    // Create private fields to hold the injected services

    private readonly AuthService _authService;

    public AuthenticateController(AuthService authService)
    {
        _authService = authService;
    }

    // Now you can use _userManager and _roleManager in your methods
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginDto loginDtoDto)
    {
        var tokens = await _authService.LogIn(loginDtoDto);
        return Ok(tokens);
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> SignUp(RegisterDto registerDto)
    {
        var tokens = await _authService.SignUp(registerDto);
        return Ok(tokens);
    }
}