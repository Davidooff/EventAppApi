using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IDatabaseContext
{
    DbSet<User> Users { get; set; }
    public DbSet<Audience> Audiences { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Speaker> Speakers { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}