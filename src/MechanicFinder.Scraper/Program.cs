using System.Text.Json;
using MechanicFinder.Scraper.Services;
using MechanicFinder.Core.Entities;
using MechanicFinder.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var databasePath = configuration["Database:Path"] ?? Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "data", "mechanicfinder.db");
var connectionString = $"Data Source={databasePath}";

var options = new DbContextOptionsBuilder<MechanicFinderDbContext>()
    .UseSqlite(connectionString)
    .Options;

await using var db = new MechanicFinderDbContext(options);
await SeedData.EnsureSeededAsync(db);

var googleApiKey = configuration["GooglePlaces:ApiKey"] ?? configuration["GooglePlaces__ApiKey"];
var googlePlacesService = new GooglePlacesService(googleApiKey);

var businesses = await db.Businesses
    .Include(b => b.City)
        .ThenInclude(c => c.Province)
    .OrderBy(b => b.Name)
    .Take(50)
    .ToListAsync();

var processed = 0;

foreach (var business in businesses)
{
    if (business.City is null)
    {
        continue;
    }

    var cityName = business.City.Name;
    var provinceName = business.City.Province?.Name ?? string.Empty;

    var placesResult = await googlePlacesService.SearchAsync(business.Name, cityName, provinceName);
    if (placesResult is null || placesResult.Rating is null)
    {
        continue;
    }

    var alreadyExists = await db.BusinessRatings.AnyAsync(r =>
        r.BusinessId == business.Id &&
        r.Source == "Google Maps" &&
        r.ObservedAtUtc.Date == placesResult.ObservedAtUtc.Date);

    if (alreadyExists)
    {
        continue;
    }

    db.BusinessRatings.Add(new BusinessRating
    {
        BusinessId = business.Id,
        Source = "Google Maps",
        RatingValue = placesResult.Rating.Value,
        RatingScaleMaximum = 5,
        ReviewCount = placesResult.ReviewCount ?? 0,
        SourceUrl = placesResult.SourceUrl,
        ObservedAtUtc = placesResult.ObservedAtUtc,
        IsCurrent = true
    });

    processed++;
}

await db.SaveChangesAsync();
Console.WriteLine($"Processed {processed} Google Places rating insertions.");
Console.WriteLine($"Google Places API key configured: {!string.IsNullOrWhiteSpace(googleApiKey)}");
