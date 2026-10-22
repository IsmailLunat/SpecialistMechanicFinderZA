using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MechanicFinder.Core.Entities;

public class Province
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    public ICollection<City> Cities { get; set; } = new List<City>();
}

public class City
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public int ProvinceId { get; set; }
    public Province Province { get; set; } = null!;

    [MaxLength(200)]
    public string? Aliases { get; set; }

    [MaxLength(100)]
    public string? LocalityType { get; set; }

    public ICollection<Business> Businesses { get; set; } = new List<Business>();
}

public class VehicleMake
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string NormalizedName { get; set; } = string.Empty;

    public ICollection<BusinessVehicleMake> BusinessVehicleMakes { get; set; } = new List<BusinessVehicleMake>();
}

public class SpecialtyCategory
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public ICollection<BusinessSpecialty> BusinessSpecialties { get; set; } = new List<BusinessSpecialty>();
}

public class Business
{
    public int Id { get; set; }

    [Required, MaxLength(250)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(250)]
    public string NormalizedName { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [MaxLength(80)]
    public string? Phone { get; set; }

    [MaxLength(250)]
    public string? WebsiteUrl { get; set; }

    [MaxLength(250)]
    public string? GoogleMapsUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastVerifiedAtUtc { get; set; }

    public ICollection<BusinessVehicleMake> BusinessVehicleMakes { get; set; } = new List<BusinessVehicleMake>();
    public ICollection<BusinessSpecialty> BusinessSpecialties { get; set; } = new List<BusinessSpecialty>();
    public ICollection<BusinessRating> Ratings { get; set; } = new List<BusinessRating>();
    public ICollection<BusinessSource> Sources { get; set; } = new List<BusinessSource>();
    public ICollection<BusinessEvidence> Evidences { get; set; } = new List<BusinessEvidence>();
}

public class BusinessVehicleMake
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public int VehicleMakeId { get; set; }
    public VehicleMake VehicleMake { get; set; } = null!;

    [MaxLength(100)]
    public string EvidenceStatus { get; set; } = "Unverified";

    [MaxLength(1000)]
    public string? EvidenceNotes { get; set; }
}

public class BusinessSpecialty
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public int SpecialtyCategoryId { get; set; }
    public SpecialtyCategory SpecialtyCategory { get; set; } = null!;

    [MaxLength(100)]
    public string EvidenceStatus { get; set; } = "Unverified";

    public double ConfidenceScore { get; set; }

    [MaxLength(1000)]
    public string? EvidenceNotes { get; set; }
}

public class BusinessRating
{
    public int Id { get; set; }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    [MaxLength(120)]
    public string Source { get; set; } = string.Empty;

    public decimal RatingValue { get; set; }
    public int RatingScaleMaximum { get; set; } = 5;
    public int ReviewCount { get; set; }
    public DateTime ObservedAtUtc { get; set; } = DateTime.UtcNow;

    [MaxLength(400)]
    public string? SourceUrl { get; set; }

    public bool IsCurrent { get; set; } = true;
}

public class BusinessSource
{
    public int Id { get; set; }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    [MaxLength(120)]
    public string SourceName { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? SourceUrl { get; set; }

    [MaxLength(200)]
    public string? ExternalBusinessId { get; set; }

    public DateTime FirstSeenAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastSeenAtUtc { get; set; } = DateTime.UtcNow;

    [MaxLength(80)]
    public string SourceType { get; set; } = "Curated";

    [MaxLength(80)]
    public string VerificationStatus { get; set; } = "Unverified";
}

public class BusinessEvidence
{
    public int Id { get; set; }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    [MaxLength(120)]
    public string EvidenceType { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string EvidenceText { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? EvidenceUrl { get; set; }

    public int? RelatedVehicleMakeId { get; set; }
    public VehicleMake? RelatedVehicleMake { get; set; }

    public int? RelatedSpecialtyCategoryId { get; set; }
    public SpecialtyCategory? RelatedSpecialtyCategory { get; set; }

    public DateTime CollectedAtUtc { get; set; } = DateTime.UtcNow;
    public double ConfidenceScore { get; set; }
}

public enum SearchResultClassification
{
    General = 0,
    ProbableSpecialist = 1,
    ConfirmedSpecialist = 2
}

public class RankedBusiness
{
    public Business Business { get; set; } = null!;
    public SearchResultClassification Classification { get; set; }
    public double RelevanceScore { get; set; }
    public double EvidenceScore { get; set; }
    public decimal RatingValue { get; set; }
    public int ReviewCount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class BusinessSearchRequest
{
    public int ProvinceId { get; set; }
    public int CityId { get; set; }
    public int? VehicleMakeId { get; set; }
    public int? SpecialtyCategoryId { get; set; }
    public string SearchMode { get; set; } = "All";
}
