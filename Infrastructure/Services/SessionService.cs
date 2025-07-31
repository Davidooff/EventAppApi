using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Options;
using Infrastructure.Models;
using Infrastructure.Redis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class SessionService: ISessionService
{
    private readonly ITokenGenerator _tokenService;
    private IDatabase _redis;
    private JwtOptions _jwtOptions;
    private readonly RedisSessionsService _redisSessionsService;
    private readonly RedisUserService _redisUserService;
    

    public SessionService(ITokenGenerator tokenService,
        IConnectionMultiplexer muxer, 
        IOptions<JwtOptions> jwtOptions,
        RedisSessionsService redisSessionsService,
        RedisUserService redisUserService)
    {
        _tokenService = tokenService;
        _redis = muxer.GetDatabase();
        _jwtOptions = jwtOptions.Value;
        _redisSessionsService = redisSessionsService;
        _redisUserService = redisUserService;
    }
    
    public async Task<AuthKeyPairDto> Create(int userId)
    {
        var session = new Sessions()
        {
            UserId = userId,
        };
        
        var sessionId = await _redisSessionsService.SetSession(session);

        
        return _tokenService.GenerateTokens(userId, sessionId);
    }

    public async Task<AuthKeyPairDto> Refresh(string sessionId)
    {
        var session = await _redisSessionsService.GetSession(sessionId);
        if (session == null)
            throw new InvalidTokenException();
        
        await _redisSessionsService.SetSession(session, sessionId);
        return _tokenService.GenerateTokens(session.UserId, sessionId);
    }

    public Task Delete(string sessionId)
    {
        return _redisSessionsService.DeleteSession(sessionId);
    }

    public async Task DeleteAllSessions(string sessionId)
    {
        var currentSessionHash = await _redis.HashGetAllAsync($"session:{sessionId}");
        if (currentSessionHash.Length == 0)
            throw new InvalidTokenException();
        var currentSession = currentSessionHash.ConvertFromHash<Sessions>();
        
        RedisValue[] sessionIds = await _redis.SetMembersAsync($"idx:session:userid:{currentSession.UserId}");

        foreach (var userSessionId in sessionIds)
        {
            await _redis.KeyDeleteAsync($"session:{sessionId}").ConfigureAwait(false);
        }
    }
}