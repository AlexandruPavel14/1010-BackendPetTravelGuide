using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetTravelGuide.Backend.Database;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    private readonly DatabaseContext _context;

    public LocationsController(DatabaseContext context)
    {
        _context = context;
    }

    // GET: api/Locations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetLocations(
        string? county = null,
        string? city = null)
    {
        if (!string.IsNullOrEmpty(county) && !string.IsNullOrEmpty(city))
        {
            return BadRequest("Please provide either the county or city, not both.");
        }

        var query = _context.Items.AsQueryable();

        if (!string.IsNullOrEmpty(county))
        {
            query = query.Where(item => item.County == county);
        }

        if (!string.IsNullOrEmpty(city))
        {
            query = query.Where(item => item.City == city);
        }

        var locations = await query.ToListAsync();

        return Ok(locations);
    }
}
