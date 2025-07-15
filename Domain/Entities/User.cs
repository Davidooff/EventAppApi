using Domain.Entities;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string Img {get; set;}

    public string FirstName {get; set;}
    
    public string LastName {get; set;}
    public ICollection<Audience> Audiences { get; set; }
    public ICollection<Speaker> Speakers { get; set; }

}