using System.Globalization;
using MechanicFinder.Core.Entities;
using MechanicFinder.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables();

var config = builder.Build();
var databasePath = config["Database:Path"] ?? "data/mechanicfinder.db";

var options = new DbContextOptionsBuilder<MechanicFinderDbContext>()
    .UseSqlite($"Data Source={databasePath}")
    .Options;

await using var context = new MechanicFinderDbContext(options);
await SeedData.EnsureSeededAsync(context);

var argsList = args.ToList();

if (argsList.Contains("--import"))
{
    var index = argsList.IndexOf("--import");
    if (index + 1 < argsList.Count)
    {
        var csvPath = argsList[index + 1];
        var dryRun = argsList.Contains("--dry-run");
        await ImportCsvAsync(context, csvPath, dryRun);
        return;
    }
}

Console.WriteLine("Usage: dotnet run --project src/MechanicFinder.Scraper -- --import data/demo-businesses.csv [--dry-run]");

static async Task ImportCsvAsync(MechanicFinderDbContext context, string csvPath, bool dryRun)
{
    if (!File.Exists(csvPath))
    {
        Console.WriteLine($"CSV file not found: {csvPath}");
        return;
    }

    using var reader = new StreamReader(csvPath);
    var lines = await reader.ReadToEndAsync();
    var rows = lines.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

    if (rows.Length <= 1)
    {
        Console.WriteLine("No data rows to import.");
        return;
    }

    var header = rows[0].Split(',');
    var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    for (var i = 0; i < header.Length; i++)
    {
        columnMap[header[i].Trim()] = i;
    }

    var imported = 0;
    foreach (var row in rows.Skip(1))
    {
        var parts = row.Split(',');
        var businessName = parts[columnMap["BusinessName"]].Trim();
        var provinceName = parts[columnMap["Province"]].Trim();
        var cityName = parts[columnMap["City"]].Trim();
        var address = parts.Length > 3 ? parts[columnMap["Address"]].Trim() : string.Empty;
        var phone = parts.Length > 4 && columnMap.ContainsKey("Phone") ? parts[columnMap["Phone"]].Trim() : string.Empty;
        var website = parts.Length > 5 && columnMap.ContainsKey("Website") ? parts[columnMap["Website"]].Trim() : string.Empty;

        var province = await context.Provinces.FirstOrDefaultAsync(p => p.Name == provinceName);
        if (province == null)
        {
            province = new Province { Name = provinceName, Code = provinceName[..2].ToUpperInvariant() };
            context.Provinces.Add(province);
            await context.SaveChangesAsync();
        }

        var city = await context.Cities.FirstOrDefaultAsync(c => c.Name == cityName && c.ProvinceId == province.Id);
        if (city == null)
        {
            city = new City { Name = cityName, ProvinceId = province.Id, LocalityType = "Town" };
            context.Cities.Add(city);
            await context.SaveChangesAsync();
        }

        var exists = await context.Businesses.AnyAsync(b => b.Name == businessName && b.CityId == city.Id);
        if (exists)
        {
            continue;
        }

        var business = new Business
        {
            Name = businessName,
            NormalizedName = businessName.ToLowerInvariant(),
            Address = address,
            CityId = city.Id,
            Phone = phone,
            WebsiteUrl = website,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        context.Businesses.Add(business);
        imported++;

        if (!dryRun)
        {
            await context.SaveChangesAsync();
        }
    }

    if (dryRun)
    {
        Console.WriteLine($"Dry-run complete. {imported} records would be inserted.");
    }
    else
    {
        Console.WriteLine($"Import complete. {imported} business records added.");
    }
}
