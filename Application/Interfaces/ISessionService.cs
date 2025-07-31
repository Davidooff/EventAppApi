using Domain.Enums;
using Infrastructure.Models;

namespace Application.Interfaces;

public interface ISessionService
{
    Task<AuthKeyPairDto> Create(int userId);
    
    Task<AuthKeyPairDto> Refresh(string sessionId);
    
    Task Delete(string sessionId);
    
    Task DeleteAllSessions(string sessionId);
}