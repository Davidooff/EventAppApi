using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

//  https://learn.microsoft.com/en-us/ef/core/
public class DatabaseContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<Audience> Audiences { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Speaker> Speakers { get; set; }


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
        // .OnDelete(DeleteBehavior.Cascade); // configure cascade delete, when category deleted, then all events getting deleted 

        modelBuilder.Entity<Audience>()
            .HasOne(a => a.Event)
            .WithMany(e => e.Audiences)
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Audience>()
            .HasOne(a => a.User)
            .WithMany(u => u.Audiences)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
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
        
        modelBuilder.Entity<Sessions>()
            .HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}