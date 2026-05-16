using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GameBacklog.Models;

namespace GameBacklog.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);  // dôležité! Identity tabuľky

        // Konfigurácia vzťahu Game -> User
        builder.Entity<Game>()
            .HasOne(g => g.User)
            .WithMany(u => u.Games)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);  // ak sa zmaže user, zmažu sa aj jeho hry
    }
}