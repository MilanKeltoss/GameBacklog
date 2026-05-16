using Microsoft.EntityFrameworkCore;
using GameBacklog.Models;

namespace GameBacklog.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }
}