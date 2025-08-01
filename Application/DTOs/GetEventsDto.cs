namespace Application.DTOs;

public class GetEventsDto
{
    public int? CategoryId { get; set; }
    public int Skip { get; set; } = 0;
    public int Count { get; set; } = 20;
}