using MechanicFinder.Core.Entities;
using MechanicFinder.Core.Scoring;

namespace MechanicFinder.UnitTests;

public class SearchRankingServiceTests
{
    [Fact]
    public void ConfirmedVolvoSpecialist_RanksAheadOfGeneralMechanic()
    {
        var volvo = new VehicleMake { Id = 1, Name = "Volvo", NormalizedName = "volvo" };
        var electrical = new SpecialtyCategory { Id = 1, Name = "Automotive electrical systems", Slug = "automotive-electrical" };
        var city = new City { Id = 1, Name = "Cape Town", ProvinceId = 1 };

        var specialist = new Business
        {
            Id = 1,
            Name = "Volvo Specialist Cape Town",
            NormalizedName = "volvo specialist cape town",
            City = city,
            CityId = city.Id,
            Description = "Volvo specialist",
            BusinessVehicleMakes =
            {
                new BusinessVehicleMake { VehicleMakeId = volvo.Id, VehicleMake = volvo, EvidenceStatus = "Confirmed", EvidenceNotes = "Dedicated Volvo specialist" }
            },
            BusinessSpecialties =
            {
                new BusinessSpecialty { SpecialtyCategoryId = electrical.Id, SpecialtyCategory = electrical, ConfidenceScore = 0.93, EvidenceStatus = "Confirmed" }
            },
            Evidences =
            {
                new BusinessEvidence { EvidenceType = "Website claim", EvidenceText = "Volvo servicing and diagnostics", ConfidenceScore = 20, RelatedVehicleMakeId = volvo.Id, RelatedSpecialtyCategoryId = electrical.Id }
            },
            Ratings =
            {
                new BusinessRating { Source = "Google Maps", RatingValue = 4.6m, RatingScaleMaximum = 5, ReviewCount = 150, IsCurrent = true }
            }
        };

        var general = new Business
        {
            Id = 2,
            Name = "General Works Cape Town",
            NormalizedName = "general works cape town",
            City = city,
            CityId = city.Id,
            Description = "General workshop",
            Ratings =
            {
                new BusinessRating { Source = "Google Maps", RatingValue = 4.9m, RatingScaleMaximum = 5, ReviewCount = 400, IsCurrent = true }
            }
        };

        var service = new SearchRankingService();
        var result = service.Rank(new[] { general, specialist }, volvo.Id, electrical.Id);

        Assert.Equal("Volvo Specialist Cape Town", result[0].Business.Name);
        Assert.True(result[0].Classification == SearchResultClassification.ConfirmedSpecialist);
    }

    [Fact]
    public void Specialist_Classification_TakesPrecedence_OverGeneralClassification()
    {
        var make = new VehicleMake { Id = 1, Name = "Volvo", NormalizedName = "volvo" };
        var city = new City { Id = 1, Name = "Cape Town", ProvinceId = 1 };
        var specialist = new Business
        {
            Id = 1,
            Name = "Specialist Volvo Service",
            NormalizedName = "specialist volvo service",
            City = city,
            CityId = city.Id,
            BusinessVehicleMakes =
            {
                new BusinessVehicleMake { VehicleMakeId = make.Id, VehicleMake = make, EvidenceStatus = "Confirmed", EvidenceNotes = "Specialist" }
            },
            Evidences =
            {
                new BusinessEvidence { EvidenceType = "Website claim", EvidenceText = "Volvo specialist", ConfidenceScore = 10, RelatedVehicleMakeId = make.Id }
            }
        };

        var ranking = new SearchRankingService();
        var ranked = ranking.Rank(new[] { specialist }, make.Id, null);

        Assert.Equal(SearchResultClassification.ProbableSpecialist, ranked[0].Classification);
        Assert.True((int)ranked[0].Classification > (int)SearchResultClassification.General);
    }

    [Fact]
    public void EqualSpecialists_AreOrderedByDescendingRating()
    {
        var make = new VehicleMake { Id = 1, Name = "Volvo", NormalizedName = "volvo" };
        var city = new City { Id = 1, Name = "Cape Town", ProvinceId = 1 };

        var lowerRated = new Business
        {
            Id = 1,
            Name = "Lower Rated Specialist",
            NormalizedName = "lower rated specialist",
            City = city,
            CityId = city.Id,
            BusinessVehicleMakes = { new BusinessVehicleMake { VehicleMakeId = make.Id, VehicleMake = make, EvidenceStatus = "Confirmed", EvidenceNotes = "Specialist" } },
            Evidences = { new BusinessEvidence { EvidenceType = "Website", EvidenceText = "Volvo workshop", ConfidenceScore = 12, RelatedVehicleMakeId = make.Id } },
            Ratings = { new BusinessRating { Source = "Google Maps", RatingValue = 4.2m, RatingScaleMaximum = 5, ReviewCount = 100, IsCurrent = true } }
        };

        var higherRated = new Business
        {
            Id = 2,
            Name = "Higher Rated Specialist",
            NormalizedName = "higher rated specialist",
            City = city,
            CityId = city.Id,
            BusinessVehicleMakes = { new BusinessVehicleMake { VehicleMakeId = make.Id, VehicleMake = make, EvidenceStatus = "Confirmed", EvidenceNotes = "Specialist" } },
            Evidences = { new BusinessEvidence { EvidenceType = "Website", EvidenceText = "Volvo workshop", ConfidenceScore = 12, RelatedVehicleMakeId = make.Id } },
            Ratings = { new BusinessRating { Source = "Google Maps", RatingValue = 4.8m, RatingScaleMaximum = 5, ReviewCount = 200, IsCurrent = true } }
        };

        var ranking = new SearchRankingService();
        var ordered = ranking.Rank(new[] { lowerRated, higherRated }, make.Id, null);

        Assert.Equal("Higher Rated Specialist", ordered[0].Business.Name);
    }

    [Fact]
    public void MissingRating_IsHandledWithoutCrashing()
    {
        var make = new VehicleMake { Id = 1, Name = "Volvo", NormalizedName = "volvo" };
        var city = new City { Id = 1, Name = "Cape Town", ProvinceId = 1 };
        var business = new Business
        {
            Id = 1,
            Name = "No Rating Specialist",
            NormalizedName = "no rating specialist",
            City = city,
            CityId = city.Id,
            BusinessVehicleMakes = { new BusinessVehicleMake { VehicleMakeId = make.Id, VehicleMake = make, EvidenceStatus = "Confirmed", EvidenceNotes = "Strong evidence" } },
            Evidences = { new BusinessEvidence { EvidenceType = "Website", EvidenceText = "Volvo specialist", ConfidenceScore = 14, RelatedVehicleMakeId = make.Id } }
        };

        var ranking = new SearchRankingService();
        var result = ranking.Rank(new[] { business }, make.Id, null);

        Assert.Equal("No Rating Specialist", result[0].Business.Name);
        Assert.Equal(0m, result[0].RatingValue);
    }
}
