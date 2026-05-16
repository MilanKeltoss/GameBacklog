using GameBacklog.Services.Dtos;

namespace GameBacklog.Services;

public interface IRawgService
{
    Task<List<RawgGame>> SearchGamesAsync(string query);
}

public class RawgService : IRawgService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<RawgService> _logger;

    public RawgService(HttpClient httpClient, IConfiguration configuration, ILogger<RawgService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Rawg:ApiKey"]
            ?? throw new InvalidOperationException("RAWG API key not configured. Run: dotnet user-secrets set \"Rawg:ApiKey\" \"YOUR_KEY\"");
        _logger = logger;
    }

    public async Task<List<RawgGame>> SearchGamesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<RawgGame>();
        }

        try
        {
            var url = $"games?key={_apiKey}&search={Uri.EscapeDataString(query)}&page_size=8";
            var response = await _httpClient.GetFromJsonAsync<RawgSearchResponse>(url);
            return response?.Results ?? new List<RawgGame>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling RAWG API for query: {Query}", query);
            return new List<RawgGame>();
        }
    }
}