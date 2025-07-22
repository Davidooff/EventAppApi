using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Models;

namespace Infrastructure.Services;

public class SessionService: ISessionService
{
    private readonly IDatabaseContext _context; 
    private readonly ITokenGenerator _tokenService;
    private readonly Random _random = new();

    private int GenUid()
    {
        return _random.Next(int.MinValue, int.MaxValue);
    }

    public SessionService(ITokenGenerator tokenService, IDatabaseContext context)
    {
        _tokenService = tokenService;
        _context = context;
    }
    
    public async Task<AuthKeyPairDto> Create(int userId)
    {
        var newSession = new Sessions
        {
            KeyUidPayload = GenUid(),
            UserId = userId,
            ExpireAt = DateTime.UtcNow.AddMinutes(_tokenService.JwtOptions.RefreshTokenExpiration)
        };
        _context.Sessions.Add(newSession);
        await _context.SaveChangesAsync();
        return _tokenService.GenerateTokens(userId, newSession.Id, newSession.KeyUidPayload);
    }

    public async Task<AuthKeyPairDto> Refresh(int sessionId, int keyUidPayload)
    {
        var currentSession = await _context.Sessions.FindAsync(sessionId);
        
        if (currentSession == null || currentSession.KeyUidPayload == keyUidPayload)
            throw new InvalidTokenException();

        currentSession.KeyUidPayload = GenUid();
        currentSession.ExpireAt = DateTime.UtcNow.AddMinutes(_tokenService.JwtOptions.RefreshTokenExpiration);
        await _context.SaveChangesAsync();
        
        return _tokenService.GenerateTokens(currentSession.UserId, currentSession.Id, currentSession.KeyUidPayload);
    }

    public async Task Delete(int sessionId, int keyUidPayload)
    {
        var currentSession = await _context.Sessions.FindAsync(sessionId);
        
        if (currentSession == null || currentSession.KeyUidPayload == keyUidPayload)
            throw new InvalidTokenException();
        
        _context.Sessions.Remove(currentSession);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAllSessions(int userId, int sessionId)
    {
        throw new NotImplementedException();
    }
}