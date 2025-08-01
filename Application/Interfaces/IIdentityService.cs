using Domain.Entities;
using WebApplication1.Dto;

namespace Application.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, User user)> CheckPasswordByEmailAsync(string email, string password);
    Task<bool> CheckPasswordByUserAsync(User user, string password);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> CreateAsync(RegisterDto registerDtoDto);
}