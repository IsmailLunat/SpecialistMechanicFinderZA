@model MechanicFinder.Web.Controllers.SearchPageViewModel
@{
    ViewData["Title"] = "Search results";
}

<h2>Search results</h2>
@if (Model.Results.SpecialistResults.Any())
{
    <div class="section-box">
        <h3>Specialist service centres</h3>
        @foreach (var result in Model.Results.SpecialistResults)
        {
            <div class="result-card">
                <div class="badge specialist">@result.SpecialistLabel</div>
                <h4>@result.Business.Name</h4>
                <div class="meta">@result.Business.City.Name · @result.Business.Address</div>
                <div class="meta">Rating: @result.RatingValue.ToString("0.0")/5 from @result.ReviewCount reviews</div>
                <p>@result.Business.Description</p>
                <p>@result.Reason</p>
            </div>
        }
    </div>
}

@if (Model.Results.GeneralResults.Any())
{
    <div class="section-box">
        <h3>General service centres</h3>
        @foreach (var result in Model.Results.GeneralResults)
        {
            <div class="result-card">
                <div class="badge general">General service centre</div>
                <h4>@result.Business.Name</h4>
                <div class="meta">@result.Business.City.Name · @result.Business.Address</div>
                <div class="meta">Rating: @result.RatingValue.ToString("0.0")/5 from @result.ReviewCount reviews</div>
                <p>@result.Business.Description</p>
            </div>
        }
    </div>
}

@if (!Model.Results.SpecialistResults.Any() && !Model.Results.GeneralResults.Any())
{
    <div class="empty-state">@Model.Results.Message</div>
}
