namespace Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public string Img { get; set; }
    public string Banner { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<Audience> Audiences { get; set; }
    public ICollection<Speaker> Speakers { get; set; }
}