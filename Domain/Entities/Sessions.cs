namespace Domain.Entities;

public class Sessions
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}