using MechanicFinder.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MechanicFinder.Infrastructure.Data;

public static class SeedData
{
    public static async Task EnsureSeededAsync(MechanicFinderDbContext context, CancellationToken cancellationToken = default)
    {
        await context.Database.EnsureCreatedAsync(cancellationToken);

        if (await context.Provinces.AnyAsync(cancellationToken))
        {
            return;
        }

        var provinces = new[]
        {
            new Province { Name = "Eastern Cape", Code = "EC" },
            new Province { Name = "Free State", Code = "FS" },
            new Province { Name = "Gauteng", Code = "GP" },
            new Province { Name = "KwaZulu-Natal", Code = "KZN" },
            new Province { Name = "Limpopo", Code = "LP" },
            new Province { Name = "Mpumalanga", Code = "MP" },
            new Province { Name = "Northern Cape", Code = "NC" },
            new Province { Name = "North West", Code = "NW" },
            new Province { Name = "Western Cape", Code = "WC" }
        };

        context.Provinces.AddRange(provinces);
        await context.SaveChangesAsync(cancellationToken);

        var provinceLookup = provinces.ToDictionary(p => p.Name, p => p.Id, StringComparer.OrdinalIgnoreCase);

        var cityRows = new[]
        {
            ("Eastern Cape", "Gqeberha"),
            ("Eastern Cape", "East London"),
            ("Eastern Cape", "Port Alfred"),
            ("Free State", "Bloemfontein"),
            ("Free State", "Welkom"),
            ("Free State", "Parys"),
            ("Gauteng", "Johannesburg"),
            ("Gauteng", "Pretoria"),
            ("Gauteng", "Soweto"),
            ("KwaZulu-Natal", "Durban"),
            ("KwaZulu-Natal", "Pietermaritzburg"),
            ("KwaZulu-Natal", "Richards Bay"),
            ("Limpopo", "Polokwane"),
            ("Limpopo", "Tzaneen"),
            ("Limpopo", "Musina"),
            ("Mpumalanga", "Mbombela"),
            ("Mpumalanga", "Nelspruit"),
            ("Mpumalanga", "Witbank"),
            ("Northern Cape", "Kimberley"),
            ("Northern Cape", "Upington"),
            ("Northern Cape", "Kuruman"),
            ("North West", "Mahikeng"),
            ("North West", "Rustenburg"),
            ("North West", "Potchefstroom"),
            ("Western Cape", "Cape Town"),
            ("Western Cape", "Stellenbosch"),
            ("Western Cape", "George")
        };

        var cities = cityRows.Select(row => new City
        {
            Name = row.Item2,
            ProvinceId = provinceLookup[row.Item1],
            LocalityType = "Town"
        }).ToList();

        context.Cities.AddRange(cities);
        await context.SaveChangesAsync(cancellationToken);

        var makes = new[]
        {
            "Volvo", "BMW", "Mercedes-Benz", "Volkswagen", "Toyota", "Ford", "Audi", "Land Rover", "Porsche", "Honda"
        };

        foreach (var make in makes)
        {
            context.VehicleMakes.Add(new VehicleMake
            {
                Name = make,
                NormalizedName = make.Trim()
            });
        }

        await context.SaveChangesAsync(cancellationToken);

        var specialties = new[]
        {
            new SpecialtyCategory { Name = "Manufacturer-specific servicing and repairs", Slug = "manufacturer-servicing", Description = "Workshop expertise focused on a specific manufacturer or model." },
            new SpecialtyCategory { Name = "Automatic and manual transmissions", Slug = "transmissions", Description = "Transmission diagnosis and rebuilds." },
            new SpecialtyCategory { Name = "Diesel engines", Slug = "diesel-engines", Description = "Diesel diagnostics and repair." },
            new SpecialtyCategory { Name = "Engine rebuilding", Slug = "engine-rebuilding", Description = "Engine overhaul and rebuild services." },
            new SpecialtyCategory { Name = "Automotive electrical systems", Slug = "automotive-electrical", Description = "Electrical diagnosis and repair." },
            new SpecialtyCategory { Name = "ECU diagnostics and programming", Slug = "ecu-diagnostics", Description = "Electronic control unit programming and fault tracing." },
            new SpecialtyCategory { Name = "Air-conditioning systems", Slug = "air-conditioning", Description = "A/C diagnosis, repair, and gas recharge." },
            new SpecialtyCategory { Name = "Gearbox rebuilding", Slug = "gearbox-rebuilding", Description = "Gearbox repair and rebuilds." },
            new SpecialtyCategory { Name = "Turbochargers", Slug = "turbochargers", Description = "Turbocharger diagnosis and rebuilds." },
            new SpecialtyCategory { Name = "Suspension and steering", Slug = "suspension-steering", Description = "Suspension and steering repairs." },
            new SpecialtyCategory { Name = "Brakes", Slug = "brakes", Description = "Brake system diagnosis and replacement." },
            new SpecialtyCategory { Name = "Hybrid and electric vehicles", Slug = "hybrid-electric", Description = "Hybrid and EV technical work." },
            new SpecialtyCategory { Name = "Classic and performance vehicles", Slug = "classic-performance", Description = "Classic or performance vehicle specialist work." }
        };

        context.SpecialtyCategories.AddRange(specialties);
        await context.SaveChangesAsync(cancellationToken);

        var capeTown = await context.Cities.SingleAsync(c => c.Name == "Cape Town", cancellationToken);
        var johannesburg = await context.Cities.SingleAsync(c => c.Name == "Johannesburg", cancellationToken);
        var durban = await context.Cities.SingleAsync(c => c.Name == "Durban", cancellationToken);
        var pretoria = await context.Cities.SingleAsync(c => c.Name == "Pretoria", cancellationToken);

        var volvo = await context.VehicleMakes.SingleAsync(v => v.Name == "Volvo", cancellationToken);
        var bmw = await context.VehicleMakes.SingleAsync(v => v.Name == "BMW", cancellationToken);
        var mercedes = await context.VehicleMakes.SingleAsync(v => v.Name == "Mercedes-Benz", cancellationToken);
        var general = await context.SpecialtyCategories.SingleAsync(s => s.Name == "Automotive electrical systems", cancellationToken);

        context.Businesses.AddRange(
            new Business
            {
                Name = "Demo Volvo Specialist Cape Town",
                NormalizedName = "demo volvo specialist cape town",
                Description = "Demo data only. This fictional workshop is included to demonstrate specialist ranking behavior.",
                Address = "14 Main Road, Cape Town",
                CityId = capeTown.Id,
                Phone = "021 555 0101",
                WebsiteUrl = "https://example.com/volvo-demo",
                IsActive = true,
                LastVerifiedAtUtc = DateTime.UtcNow.AddDays(-7),
                BusinessVehicleMakes =
                {
                    new BusinessVehicleMake
                    {
                        VehicleMakeId = volvo.Id,
                        EvidenceStatus = "Confirmed",
                        EvidenceNotes = "Dedicated Volvo service page and evidence of specialist tooling."
                    }
                },
                BusinessSpecialties =
                {
                    new BusinessSpecialty
                    {
                        SpecialtyCategoryId = general.Id,
                        ConfidenceScore = 0.92,
                        EvidenceStatus = "Confirmed",
                        EvidenceNotes = "Workshop page clearly states Volvo diagnostics and electrical expertise."
                    }
                },
                Ratings =
                {
                    new BusinessRating
                    {
                        Source = "Google Maps",
                        RatingValue = 4.6m,
                        RatingScaleMaximum = 5,
                        ReviewCount = 154,
                        ObservedAtUtc = DateTime.UtcNow.AddDays(-3),
                        IsCurrent = true
                    }
                },
                Evidences =
                {
                    new BusinessEvidence
                    {
                        EvidenceType = "Workshop description",
                        EvidenceText = "Volvo servicing and diagnostics specialist workshop.",
                        ConfidenceScore = 0.95,
                        RelatedVehicleMakeId = volvo.Id,
                        RelatedSpecialtyCategoryId = general.Id
                    }
                }
            },
            new Business
            {
                Name = "Demo General Service Hub Johannesburg",
                NormalizedName = "demo general service hub johannesburg",
                Description = "Demo data only. General workshop used to demonstrate general classification in results.",
                Address = "88 Buitenkant Road, Johannesburg",
                CityId = johannesburg.Id,
                Phone = "011 555 2222",
                WebsiteUrl = "https://example.com/general-demo",
                IsActive = true,
                LastVerifiedAtUtc = DateTime.UtcNow.AddDays(-10),
                BusinessVehicleMakes =
                {
                    new BusinessVehicleMake
                    {
                        VehicleMakeId = bmw.Id,
                        EvidenceStatus = "Unverified",
                        EvidenceNotes = "Repairs multiple vehicle brands, no dedicated specialist evidence."
                    }
                },
                Ratings =
                {
                    new BusinessRating
                    {
                        Source = "Google Maps",
                        RatingValue = 4.9m,
                        RatingScaleMaximum = 5,
                        ReviewCount = 320,
                        ObservedAtUtc = DateTime.UtcNow.AddDays(-4),
                        IsCurrent = true
                    }
                }
            },
            new Business
            {
                Name = "Demo Mercedes Tech Durban",
                NormalizedName = "demo mercedes tech durban",
                Description = "Demo data only. A Mercedes-focused workshop demonstrating specialist filtering.",
                Address = "45 Long Street, Durban",
                CityId = durban.Id,
                Phone = "031 555 3311",
                WebsiteUrl = "https://example.com/mercedes-demo",
                IsActive = true,
                LastVerifiedAtUtc = DateTime.UtcNow.AddDays(-2),
                BusinessVehicleMakes =
                {
                    new BusinessVehicleMake
                    {
                        VehicleMakeId = mercedes.Id,
                        EvidenceStatus = "Confirmed",
                        EvidenceNotes = "Mercedes specialist services and diagnostics expertise."
                    }
                }
            },
            new Business
            {
                Name = "Demo Pretoria Auto Works",
                NormalizedName = "demo pretoria auto works",
                Description = "Demo data only. A general service centre with a high rating but no specialist evidence.",
                Address = "12 Voortrekker Road, Pretoria",
                CityId = pretoria.Id,
                Phone = "012 555 1212",
                WebsiteUrl = "https://example.com/pretoria-demo",
                IsActive = true,
                LastVerifiedAtUtc = DateTime.UtcNow.AddDays(-6),
                Ratings =
                {
                    new BusinessRating
                    {
                        Source = "Google Maps",
                        RatingValue = 4.7m,
                        RatingScaleMaximum = 5,
                        ReviewCount = 200,
                        ObservedAtUtc = DateTime.UtcNow.AddDays(-3),
                        IsCurrent = true
                    }
                }
            }
        );

        await context.SaveChangesAsync(cancellationToken);
    }
}
