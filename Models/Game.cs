using System.ComponentModel.DataAnnotations;

namespace GameBacklog.Models;

public class Game
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Platform")]
    public string Platform { get; set; } = string.Empty;

    [Display(Name = "Genre")]
    public string? Genre { get; set; }

    [Required]
    [Display(Name = "Status")]
    public GameStatus Status { get; set; }

    [Range(1, 10)]
    [Display(Name = "Rating")]
    public int? Rating { get; set; }

    [Display(Name = "Date Added")]
    public DateTime DateAdded { get; set; } = DateTime.Now;

    [Display(Name = "Cover Image URL")]
    public string? CoverImageUrl { get; set; }

    [Display(Name = "RAWG ID")]
    public int? RawgId { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    // Foreign key to ApplicationUser
    public string UserId { get; set; } = string.Empty;

    // Navigation property
    public ApplicationUser? User { get; set; }
}

public enum GameStatus
{
    [Display(Name = "Want to Play")]
    WantToPlay,

    [Display(Name = "Playing")]
    Playing,

    [Display(Name = "Completed")]
    Completed,

    [Display(Name = "Dropped")]
    Dropped
}