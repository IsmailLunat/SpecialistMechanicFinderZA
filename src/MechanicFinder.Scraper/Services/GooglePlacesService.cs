using System.Text.Json;

namespace MechanicFinder.Scraper.Services;

public sealed class GooglePlacesResult
{
    public string BusinessName { get; set; } = string.Empty;
    public string? PlaceId { get; set; }
    public decimal? Rating { get; set; }
    public int? ReviewCount { get; set; }
    public string? SourceUrl { get; set; }
    public DateTime ObservedAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class GooglePlacesService
{
    private readonly string? _apiKey;
    private readonly HttpClient _httpClient;

    public GooglePlacesService(string? apiKey, HttpClient? httpClient = null)
    {
        _apiKey = apiKey;
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<GooglePlacesResult?> SearchAsync(
        string businessName,
        string city,
        string province,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(businessName))
        {
            return null;
        }

        var query = BuildQuery(businessName, city, province);
        var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(query)}&key={Uri.EscapeDataString(_apiKey)}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (!root.TryGetProperty("status", out var statusElement))
        {
            return null;
        }

        var status = statusElement.GetString();
        if (status is null || status != "OK")
        {
            return null;
        }

        if (!root.TryGetProperty("results", out var resultsElement) || resultsElement.ValueKind != JsonValueKind.Array || resultsElement.GetArrayLength() == 0)
        {
            return null;
        }

        var firstResult = resultsElement[0];
        var resultName = firstResult.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : businessName;

        decimal? rating = null;
        if (firstResult.TryGetProperty("rating", out var ratingElement) && ratingElement.ValueKind == JsonValueKind.Number)
        {
            rating = ratingElement.GetDecimal();
        }

        int? reviewCount = null;
        if (firstResult.TryGetProperty("user_ratings_total", out var reviewCountElement) && reviewCountElement.ValueKind == JsonValueKind.Number)
        {
            reviewCount = reviewCountElement.GetInt32();
        }

        string? placeId = null;
        if (firstResult.TryGetProperty("place_id", out var placeIdElement))
        {
            placeId = placeIdElement.GetString();
        }

        var sourceUrl = $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(resultName ?? businessName)}";

        return new GooglePlacesResult
        {
            BusinessName = resultName ?? businessName,
            PlaceId = placeId,
            Rating = rating,
            ReviewCount = reviewCount,
            SourceUrl = sourceUrl,
            ObservedAtUtc = DateTime.UtcNow
        };
    }

    private static string BuildQuery(string businessName, string city, string province)
    {
        var parts = new List<string>
        {
            businessName.Trim(),
            city.Trim(),
            province.Trim(),
            "South Africa"
        };

        return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }
}
