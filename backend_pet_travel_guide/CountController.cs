using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace YourNamespace.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public LocationsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("count")]
        public IActionResult GetLocationCount()
        {
            var locationCount = _dbContext.Locations.Count();
            return Ok(new { Count = locationCount });
        }

        [HttpGet("count-by-county")]
        public IActionResult GetLocationCountByCounty()
        {
            var locationCountByCounty = _dbContext.Locations
                .GroupBy(l => l.county)
                .Select(g => new { County = g.Key, Count = g.Count() })
                .ToList();

            return Ok(locationCountByCounty);
        }

// stergeere locatii
        // [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")]
        // public IActionResult DeleteLocation(int id)
        // {
        //     var location = _dbContext.Locations.Find(id);
        //     if (location == null)
        //     {
        //         return NotFound();
        //     }

        //     _dbContext.Locations.Remove(location);
        //     _dbContext.SaveChanges();
        //     return NoContent();
        // }


// afisare locatii
        // [HttpGet("{id}")]
        // public IActionResult GetLocationById(int id)
        // {
        //     var location = _dbContext.Locations.Find(id);
        //     if (location == null)
        //     {
        //         return NotFound();
        //     }
            
        //     return Ok(location);
        // }




    }
}
