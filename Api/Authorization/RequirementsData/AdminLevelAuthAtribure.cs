using Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Authorization.RequirementsData;

public class AdminLevelAuthAttribute(EUserPermissions accessLevel) : AuthorizeAttribute, 
    IAuthorizationRequirement, IAuthorizationRequirementData
{
    public EUserPermissions AccessLevel { get; set; } = accessLevel;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}
