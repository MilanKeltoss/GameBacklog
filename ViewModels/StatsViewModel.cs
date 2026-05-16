namespace GameBacklog.ViewModels;

public class StatsViewModel
{
    public int TotalGames { get; set; }
    public int WantToPlayCount { get; set; }
    public int PlayingCount { get; set; }
    public int CompletedCount { get; set; }
    public int DroppedCount { get; set; }
    public double? AverageRating { get; set; }
    public string? TopGenre { get; set; }
    public string? TopPlatform { get; set; }
    public Dictionary<string, int> GamesByPlatform { get; set; } = new();
    public Dictionary<string, int> GamesByGenre { get; set; } = new();
}