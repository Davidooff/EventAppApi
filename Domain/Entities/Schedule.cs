namespace Domain.Entities;

public class Schedule
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
}