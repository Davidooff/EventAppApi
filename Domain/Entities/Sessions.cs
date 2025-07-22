namespace Domain.Entities;

public class Sessions
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int KeyUidPayload { get; set; }
    public User User { get; set; } = null!;
    public DateTime ExpireAt { get; set; }
}