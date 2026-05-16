using System.Text.Json.Serialization;

namespace GameBacklog.Services.Dtos;

public class RawgSearchResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("results")]
    public List<RawgGame> Results { get; set; } = new();
}

public class RawgGame
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("released")]
    public string? Released { get; set; }

    [JsonPropertyName("background_image")]
    public string? BackgroundImage { get; set; }

    [JsonPropertyName("genres")]
    public List<RawgGenre> Genres { get; set; } = new();
}

public class RawgGenre
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}