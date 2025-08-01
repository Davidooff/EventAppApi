using Domain.Enums;

namespace Application.Extensions;

public static class EUserPermissionsExtensions
{
    public static List<EAdminPanels> PermittedPanels(this EUserPermissions accessLevel)
    {
        var panels = new List<EAdminPanels>();
        
        if (accessLevel >= EUserPermissions.Host)
            panels.Add(EAdminPanels.Events);
        
        if (accessLevel >= EUserPermissions.Moderator)
            panels.Add(EAdminPanels.Users);
        
        if (accessLevel >= EUserPermissions.Admin)
            panels.Add(EAdminPanels.Categories);
        
        return panels;
    }
}