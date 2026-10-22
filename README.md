using MechanicFinder.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MechanicFinder.Web.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationApiController : ControllerBase
{
    private readonly MechanicFinderDbContext _context;

    public LocationApiController(MechanicFinderDbContext context)
    {
        _context = context;
    }

    [HttpGet("cities")]
    public async Task<IActionResult> GetCities([FromQuery] int provinceId, CancellationToken cancellationToken)
    {
        if (provinceId <= 0)
        {
            return BadRequest(new { message = "Province Id must be greater than zero." });
        }

        var cities = await _context.Cities
            .Where(c => c.ProvinceId == provinceId)
            .OrderBy(c => c.Name)
            .Select(c => new { id = c.Id, name = c.Name })
            .ToListAsync(cancellationToken);

        return Ok(cities);
    }
}
