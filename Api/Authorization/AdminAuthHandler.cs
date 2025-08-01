using System.Security.Claims;
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Authorization.RequirementsData;

namespace WebApplication1.Authorization;

public class AdminAuthHandler(ILogger<AdminAuthHandler> logger, ISessions sessionService,IUserCash userService) 
    : AuthorizationHandler<AdminLevelAuthAttribute>
{
    // Check whether a given minimum age requirement is satisfied.
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        AdminLevelAuthAttribute requirement)
    {
        logger.LogInformation(
            "Evaluating authorization requirement for access level: {}", requirement.AccessLevel);

        var sessionId = context.User.FindFirst(ClaimTypes.SerialNumber)?.Value;
        var userIdStr = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        logger.LogInformation($"Evaluating authorization requirement for: session - {sessionId}, userId - {userIdStr}");
        if (sessionId == null || userIdStr == null || !int.TryParse(userIdStr, out var userId))
            return;

        if (requirement.AccessLevel == 0)
        {
            var sessionWithNoNeedToAccessLvl = await sessionService.GetSession(sessionId);
            if (sessionWithNoNeedToAccessLvl != null)
                context.Succeed(requirement);
            
            return;
        }
        
        var user = userService.GetUser(userId);
        var session = sessionService.GetSession(sessionId);
        await Task.WhenAll(user, session);
        if (session.Result == null)
            return;
        
        if (user.Result.AccessLevel >= requirement.AccessLevel) 
            context.Succeed(requirement);
    }
}
