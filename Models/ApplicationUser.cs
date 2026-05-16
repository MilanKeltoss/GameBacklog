using Microsoft.AspNetCore.Identity;

namespace GameBacklog.Models;

public class ApplicationUser : IdentityUser
{
    // Navigation property - one user has many games
    public ICollection<Game> Games { get; set; } = new List<Game>();
}