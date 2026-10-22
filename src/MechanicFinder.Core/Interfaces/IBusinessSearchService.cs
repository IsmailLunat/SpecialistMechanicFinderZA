using MechanicFinder.Core.Entities;

namespace MechanicFinder.Core.Scoring;

public class SearchRankingService
{
    public IReadOnlyList<RankedBusiness> Rank(
        IEnumerable<Business> businesses,
        int? requestedVehicleMakeId,
        int? requestedSpecialtyCategoryId)
    {
        return businesses
            .Select(b => ScoreBusiness(b, requestedVehicleMakeId, requestedSpecialtyCategoryId))
            .OrderByDescending(x => (int)x.Classification)
            .ThenByDescending(x => x.RelevanceScore)
            .ThenByDescending(x => x.EvidenceScore)
            .ThenByDescending(x => x.RatingValue)
            .ThenByDescending(x => x.ReviewCount)
            .ThenBy(x => x.Business.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static RankedBusiness ScoreBusiness(
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
            .Where(r => r.IsCurrent)
            .OrderByDescending(r => r.ObservedAtUtc)
            .ThenByDescending(r => r.RatingValue)
            .FirstOrDefault();

        var ratingValue = rating?.RatingValue ?? 0m;
        var reviewCount = rating?.ReviewCount ?? 0;

        var relevanceScore = 0.0;
        var reason = "General service centre with no strong specialist evidence in the selected make or category.";
        var classification = SearchResultClassification.General;

        if (requestedVehicleMakeId.HasValue && makeMatch is not null)
        {
            relevanceScore += SearchRankingConfiguration.ExactVehicleMatchWeight;
            reason = $"Listed as relevant to the requested vehicle make based on recorded evidence for {makeMatch.VehicleMake.Name}.";
        }

        if (requestedSpecialtyCategoryId.HasValue && specialtyMatch is not null)
        {
            relevanceScore += SearchRankingConfiguration.ExactSpecialtyMatchWeight;
            relevanceScore += specialtyMatch.ConfidenceScore * 10.0;
            reason = $"Evidence supports specialist work in the requested category: {specialtyMatch.SpecialtyCategory.Name}.";
        }

        if (requestedVehicleMakeId.HasValue && makeMatch is not null || requestedSpecialtyCategoryId.HasValue && specialtyMatch is not null)
        {
            classification = DetermineClassification(evidenceScore, makeMatch, specialtyMatch);
        }

        relevanceScore += evidenceScore * SearchRankingConfiguration.EvidenceWeight;
        if (ratingValue > 0)
        {
            relevanceScore += SearchRankingConfiguration.PublicRatingWeight;
        }

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

    private static SearchResultClassification DetermineClassification(
        double evidenceScore,
        BusinessVehicleMake? makeMatch,
        BusinessSpecialty? specialtyMatch)
    {
        var hasStrongVehicleEvidence = makeMatch is not null && !string.IsNullOrWhiteSpace(makeMatch.EvidenceNotes);
        var hasStrongSpecialtyEvidence = specialtyMatch is not null && specialtyMatch.ConfidenceScore >= 0.7;

        if (evidenceScore >= SearchRankingConfiguration.ConfirmedSpecialistThreshold || (hasStrongVehicleEvidence && hasStrongSpecialtyEvidence))
        {
            return SearchResultClassification.ConfirmedSpecialist;
        }

        if (evidenceScore >= SearchRankingConfiguration.ProbableSpecialistThreshold || hasStrongVehicleEvidence || hasStrongSpecialtyEvidence)
        {
            return SearchResultClassification.ProbableSpecialist;
        }

        return SearchResultClassification.General;
    }
}
