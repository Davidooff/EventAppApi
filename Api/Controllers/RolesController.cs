using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using Domain.Enums;
using Infrastructure.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Authorization.RequirementsData;

namespace WebApplication1.Controllers;

[ApiController]
[AdminLevelAuth(EUserPermissions.Moderator)]
[Route("roles")]
public class RolesController : ControllerBase
{
    private readonly IDatabaseContext _databaseContext;
    private readonly UserService _userService;
    public RolesController(IDatabaseContext databaseContext, UserService userService)
    {
        _databaseContext = databaseContext;
        _userService = userService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var users = await _userService.GetUsersWithAccessLevelHigherOrEqual((EUserPermissions) 1);
        return Ok(users);
    }
    
    [HttpPatch("")]
    public async Task<IActionResult> SetUserRole(ChangeUserRoleDto changeRole)
    {
        var user = await _userService.UpdateUserAccessLevel(changeRole.UserId, changeRole.NewRole);
        return Ok(user);
    }
}