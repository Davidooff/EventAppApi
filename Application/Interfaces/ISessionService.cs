using Infrastructure.Models;

namespace Application.Interfaces;

public interface ISessionService
{
    Task<AuthKeyPairDto> Create(int userId);
    
    Task<AuthKeyPairDto> Refresh(int sessionId, int keyUidPayload);
    
    Task Delete(int sessionId, int keyUidPayload);
    
    Task DeleteAllSessions(int userId, int sessionId);
}