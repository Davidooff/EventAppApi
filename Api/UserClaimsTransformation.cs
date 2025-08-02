using System.Security.Claims;
using System.Text.Json;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace WebApplication1;

public class UserClaimsTransformation(IUserCash userCash) : IClaimsTransformation
{
    
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity();
        
        var userIdSt = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if  (userIdSt == null || !int.TryParse(userIdSt, out var userId))
            return principal;
            
        var user = await userCash.GetUser(userId);
        claimsIdentity.AddClaim(new Claim(ClaimTypes.UserData, JsonSerializer.Serialize(user)));

        principal.AddIdentity(claimsIdentity);
        return principal;
    }
}