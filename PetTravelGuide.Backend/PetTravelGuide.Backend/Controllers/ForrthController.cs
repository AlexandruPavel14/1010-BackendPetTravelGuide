using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PetTravelGuide.Backend.Database;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;

        public RecommendationController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/recommendation/popularity?n=10
        [HttpGet("popularity")]
        public ActionResult<IEnumerable<Item>> GetRecommendationsByPopularity([FromQuery] int n)
        {
            if (n <= 0)
            {
                return BadRequest("Invalid value for 'n'.");
            }

            var recommendations = GetTopRatedLocations(n);
            return Ok(recommendations);
        }

        // Function to retrieve top-rated locations
        private List<Item> GetTopRatedLocations(int n)
        {
            // Sort locations by rating in descending order
            var sortedLocations = _dbContext.Items.OrderByDescending(x => x.Rating).Take(n).ToList();
            return sortedLocations;
        }
    }
}
