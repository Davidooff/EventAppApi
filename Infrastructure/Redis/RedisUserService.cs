using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using StackExchange.Redis;

namespace Infrastructure.Redis;

public class RedisUserService
{
    private readonly IConnectionMultiplexer _muxer;
    private readonly IDatabaseContext _dbContext;

    // The DI container injects the SINGLETON instance here.
    public RedisUserService(IConnectionMultiplexer muxer, IDatabaseContext dbContext)
    {
        _muxer = muxer;
        _dbContext = dbContext;
    }

    public async Task<UserCash> GetUser(int id)
    {
        IDatabase db = _muxer.GetDatabase(1);
        var session = await db.HashGetAllAsync($"user:{id}");
        if (session.Length == 0)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                throw new UserNotFoundException();
            
            await db.HashSetAsync($"user:{id}", user.ToHashEntries())
                .ConfigureAwait(false);
            
            return user.ConvertToCash();
        }
        
        return session.ConvertFromHash<UserCash>();
    }

    public async Task<bool> UpdateUser(int id, UserCash? user = null)
    {
        IDatabase db = _muxer.GetDatabase(1);
        if (user == null)
        {
            var _ = await _dbContext.Users.FindAsync(id);
            if (_ == null) 
                throw new UserNotFoundException();
            
            user = _.ConvertToCash();
        }
            
        await db.HashSetAsync($"user:{id}", user.ToHashEntries())
            .ConfigureAwait(false);
        
        return true;
    }
}