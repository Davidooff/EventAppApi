using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService
{
    private readonly IDatabaseContext _dbContext;
    private readonly IUserCash _userCash;
    public UserService(IDatabaseContext databaseContext, IUserCash userCash)
    {
        _dbContext = databaseContext;   
        _userCash = userCash;
    }

    /// <summary>
    /// Getting user by id
    /// !!! BE AWARE OF CHANGINF USER AND SAVING CONTEXT !!!
    /// !!! VALUE BEING TRACKED BY DATABASE CONTEXT !!!
    /// </summary>
    /// <param name="id">UserId</param>
    /// <returns>User</returns>
    public ValueTask<User?> GetUser(int id)
    {
        return _dbContext.Users.FindAsync(id);
    }

    public Task<User[]> GetUsers(int start, int count)
    {
        return _dbContext.Users.AsNoTracking().Skip(start).Take(count).ToArrayAsync();
    }

    public Task<User[]> Search(string email)
    {
        return _dbContext.Users.AsNoTracking().Where(u => u.Email == email).Take(20).ToArrayAsync();
    }
    
    public Task<User[]> GetUsersWithAccessLevelHigherOrEqual(EUserPermissions accessLevel)
    {
        return _dbContext.Users.AsNoTracking().Where(u => u.AccessLevel >= accessLevel).ToArrayAsync();
    }

    public async Task<User> UpdateUserAccessLevel(int userId, EUserPermissions accessLevel)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        
        if (user == null)
            throw new UserNotFoundException();
        
        user.AccessLevel = accessLevel;
        
        var isCahsed = _userCash.IsCashed(userId);
        var context =  _dbContext.SaveChangesAsync();
        await Task.WhenAll(isCahsed, context);
        
        if (isCahsed.Result)
            await _userCash.UpdateUser(userId, user.ConvertToCash());
        
        return user;
    }
}