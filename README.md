using MechanicFinder.Core.Entities;

namespace MechanicFinder.Core.Scoring;

public class SearchRankingService
{
    private const double VehicleMakeWeight = 45.0;
    private const double SpecialtyWeight = 30.0;
    private const double EvidenceWeight = 15.0;
    private const double PublicRatingWeight = 10.0;

    public IReadOnlyList<RankedBusiness> Rank(
        IEnumerable<Business> businesses,
        int? requestedVehicleMakeId,
        int? requestedSpecialtyCategoryId)
    {
        var ranked = businesses
            .Select(business => ScoreBusiness(business, requestedVehicleMakeId, requestedSpecialtyCategoryId))
            .OrderByDescending(x => x.Classification)
            .ThenByDescending(x => x.RelevanceScore)
            .ThenByDescending(x => x.EvidenceScore)
            .ThenByDescending(x => x.RatingValue)
            .ThenByDescending(x => x.ReviewCount)
            .ThenBy(x => x.Business.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return ranked;
    }

    private RankedBusiness ScoreBusiness(
        Business business,
        int? requestedVehicleMakeId,
        int? requestedSpecialtyCategoryId)
    {
        var makeMatch = business.BusinessVehicleMakes
            .FirstOrDefault(x => x.VehicleMakeId == requestedVehicleMakeId);

        var specialtyMatch = business.BusinessSpecialties
            .FirstOrDefault(x => x.SpecialtyCategoryId == requestedSpecialtyCategoryId);

        var evidenceScore = business.Evidences
            .Where(x => x.ConfidenceScore > 0)
            .Sum(x => x.ConfidenceScore);

        var rating = business.Ratings
            .Where(x => x.IsCurrent)
            .OrderByDescending(x => x.ObservedAtUtc)
            .ThenByDescending(x => x.RatingValue)
            .FirstOrDefault();

        var ratingValue = rating?.RatingValue ?? 0m;
        var reviewCount = rating?.ReviewCount ?? 0;

        var relevanceScore = 0.0;
        var reason = "General service centre with no strong specialist evidence in the requested category.";
        var classification = SearchResultClassification.General;

        if (requestedVehicleMakeId.HasValue && makeMatch is not null)
        {
            relevanceScore += VehicleMakeWeight;
            reason = $"Listed as relevant to the requested vehicle make based on recorded evidence for {makeMatch.VehicleMake.Name}.";
        }

        if (requestedSpecialtyCategoryId.HasValue && specialtyMatch is not null)
        {
            relevanceScore += SpecialtyWeight + specialtyMatch.ConfidenceScore * 10.0;
            reason = $"Evidence supports specialist work in the requested category: {specialtyMatch.SpecialtyCategory.Name}.";
        }

        if (makeMatch is not null || specialtyMatch is not null)
        {
            classification = mathToClassification(evidenceScore, makeMatch, specialtyMatch);
            if (classification == SearchResultClassification.ConfirmedSpecialist)
            {
                reason = $"Confirmed specialist evidence: {reason}";
            }
            else if (classification == SearchResultClassification.ProbableSpecialist)
            {
                reason = $"Probable specialist evidence: {reason}";
            }
        }

        relevanceScore += evidenceScore * 1.0;
        relevanceScore += ratingValue > 0 ? PublicRatingWeight : 0;

        return new RankedBusiness
        {
            Business = business,
            Classification = classification,
            RelevanceScore = relevanceScore,
            EvidenceScore = evidenceScore,
            RatingValue = ratingValue,
            ReviewCount = reviewCount,
            Reason = reason
        };
    }

    private static SearchResultClassification mathToClassification(
        double evidenceScore,
        BusinessVehicleMake? makeMatch,
        BusinessSpecialty? specialtyMatch)
    {
        var hasStrongVehicleEvidence = makeMatch is not null && !string.IsNullOrWhiteSpace(makeMatch.EvidenceNotes);
        var hasStrongSpecialtyEvidence = specialtyMatch is not null && specialtyMatch.ConfidenceScore >= 0.7;

        if (evidenceScore >= 18 || hasStrongVehicleEvidence && hasStrongSpecialtyEvidence)
        {
            return SearchResultClassification.ConfirmedSpecialist;
        }

        if (evidenceScore >= 8 || hasStrongVehicleEvidence || hasStrongSpecialtyEvidence)
        {
            return SearchResultClassification.ProbableSpecialist;
        }

        return SearchResultClassification.General;
    }
}

public static class SearchRankingConfiguration
{
    public const double ConfirmedSpecialistThreshold = 18.0;
    public const double ProbableSpecialistThreshold = 8.0;
}

public static class SearchKeywords
a{
    public const string Specialist = "Specialist";
    public const string General = "General";
}

public static class SearchClassificationExtensions
{
    public static int ToPriority(this SearchResultClassification classification) => (int)classification;
}

public static class SearchResultsText
{
    public const string SpecialistSectionTitle = "Specialist service centres";
    public const string GeneralSectionTitle = "General service centres";
}

public static class ComparisonPolicy
{
    public const string PrimaryRatingSource = "Google Maps";
}

public static class SearchModeOptions
{
    public const string All = "All";
    public const string SpecialistsOnly = "SpecialistsOnly";
    public const string GeneralOnly = "GeneralOnly";
}

public static class RankingWeights
{
    public const double ExactVehicleMakeMatch = 45;
    public const double ExactSpecialtyMatch = 30;
    public const double PublishedExpertise = 15;
    public const double PublicRating = 10;
}

public class SearchResultSummary
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public static class SearchRankingNotes
{
    public const string ConfigurableDescription = "Ranking weights and thresholds are intentionally separated from the web layer so they can be adjusted without changing controller logic.";
}

public static class SearchResultConstants
{
    public const int PageSize = 12;
}

public static class SearchResultEvidence
{
    public const string NotEnoughEvidence = "No verified specialist evidence found for the selected filters.";
}

public static class SearchResultMessages
{
    public const string NoSpecialists = "No specialist businesses matched the selected criteria. Showing general service providers in the chosen city.";
    public const string NoFindings = "No businesses matched the selected filters. Try a different city, make, or specialty.";
}

public static class StringComparer
{
    public static IComparer<string> OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase;
}
