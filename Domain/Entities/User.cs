using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<int>
{
    public string? Img { get; set; }
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public EUserPermissions AccessLevel { get; set; } = EUserPermissions.User;
    public ICollection<Audience> Audiences { get; set; }
    public ICollection<Speaker> Speakers { get; set; }

    public UserCash ConvertToCash()
    {
        return new UserCash()
        {
            AccessLevel = AccessLevel,
        };
    }
}