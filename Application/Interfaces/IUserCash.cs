using Application.Exceptions;
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserCash
{
    public Task<UserCash> GetUser(int id);
    public Task<bool> UpdateUser(int id, UserCash? user = null);
    public Task<bool> IsCashed(int id);

}