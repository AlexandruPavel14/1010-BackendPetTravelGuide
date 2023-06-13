using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Database;

namespace PetTravelGuide.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<LocationController> _logger;

        public LocationController(DatabaseContext context, ILogger<LocationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound(); // Return a 404 Not Found response if the item with the specified ID is not found
            }

            return item;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            return await _context.Items.ToListAsync();
        }

        public class RecommendedItemResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Rating { get; set; }
            public int UserRatingsTotal { get; set; }
            public int OneStar { get; set; }
            public int TwoStar { get; set; }
            public int ThreeStar { get; set; }
            public int FourStar { get; set; }
            public int FiveStar { get; set; }
            public double Distance { get; set; }
            public string Country { get; set; }
            public string County { get; set; }
            public string City { get; set; }
        }

        [HttpGet("recommended-items")]
        public async Task<ActionResult<IEnumerable<RecommendedItemResponse>>> GetRecommendedItems(double currentLatitude, double currentLongitude)
        {
            var items = await _context.Items.ToListAsync();

            var recommendedItems = items.Select(item =>
            {
                var distance = CalculateDistance(currentLatitude, currentLongitude, item.Latitude, item.Longitude);
                return new RecommendedItemResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    Rating = item.Rating,
                    UserRatingsTotal = item.UserRatingsTotal,
                    OneStar = item.OneStar,
                    TwoStar = item.TwoStar,
                    ThreeStar = item.ThreeStar,
                    FourStar = item.FourStar,
                    FiveStar = item.FiveStar,
                    Country = item.Country,
                    County = item.County,
                    City = item.City,
                    Distance = distance,
                };
            })
            .OrderBy(item => item.Distance)
            .Take(10)
            .ToList();

            return recommendedItems;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var earthRadius = 6371;
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = earthRadius * c;

            return distance;
        }


        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}

