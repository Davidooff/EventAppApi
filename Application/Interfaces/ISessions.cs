using Domain.Entities;

namespace Application.Interfaces;

public interface ISessions
{
    public Task<string> SetSession(Sessions session, string? id = null);

    public Task<Sessions?> GetSession(string id);

    public Task<bool> RemoveSession(string id);

    public Task<bool> RemoveSessionByUserId(string userId);
}