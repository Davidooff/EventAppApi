namespace Domain.Entities;

public class Audience
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; }

}