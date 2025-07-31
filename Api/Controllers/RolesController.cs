using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
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
    private readonly RedisUserService _userService;
    public RolesController(IDatabaseContext databaseContext, RedisUserService userService)
    {
        _databaseContext = databaseContext;
        _userService = userService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var users = await _databaseContext.Users.Where(u => u.AccessLevel > 0).ToArrayAsync();
        return Ok(users);
    }
    
    [HttpPatch("")]
    public async Task<IActionResult> SetUserRole(ChangeUserRoleDto changeRole)
    {
        var user = await _databaseContext.Users.FindAsync(changeRole.UserId);
        
        if (user == null)
            throw new UserNotFoundException();

        _userService.UpdateUser(changeRole.UserId);
        user.AccessLevel = changeRole.NewRole;
        _databaseContext.Users.Update(user);
        await _databaseContext.SaveChangesAsync();
        return Ok();
    }
}