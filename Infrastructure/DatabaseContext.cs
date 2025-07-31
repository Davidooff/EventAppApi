using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

//  https://learn.microsoft.com/en-us/ef/core/
public class DatabaseContext : DbContext, IDatabaseContext
{
    public DbSet<Audience> Audiences { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Speaker> Speakers { get; set; }
    public DbSet<User> Users { get; set; }
    


    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>()
            .HasOne(e => e.Category) // Each Event has one Category
            .WithMany(c => c.Events) // Each Category has many Events
            .HasForeignKey(e => e.CategoryId); // The foreign key is Event.CategoryId
            // configure cascade delete, when category deleted, then all events getting deleted 

        modelBuilder.Entity<Audience>()
            .HasOne(a => a.Event)
            .WithMany(e => e.Audiences)
            .HasForeignKey(e => e.EventId);

        modelBuilder.Entity<Audience>()
            .HasOne(a => a.User)
            .WithMany(u => u.Audiences)
            .HasForeignKey(e => e.UserId);
        
        modelBuilder.Entity<Speaker>()
            .HasOne(a => a.Event)
            .WithMany(e => e.Speakers)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Speaker>()
            .HasOne(a => a.User)
            .WithMany(u => u.Speakers)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Audiences)
            .WithOne(a => a.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Speakers)
            .WithOne(s => s.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}