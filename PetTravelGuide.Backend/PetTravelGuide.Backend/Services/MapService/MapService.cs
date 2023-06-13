using Microsoft.EntityFrameworkCore;
using PetTravelGuide.Backend.Database;
using PetTravelGuide.Backend.Models.Map;

namespace PetTravelGuide.Backend.Services.MapService;

public class MapService : IMapService
{
    private readonly DatabaseContext _database;

    public MapService(DatabaseContext database)
    {
        _database = database;
    }
    
    public async Task<List<MapLocation>> GetLocationMarkers()
    {
        return await _database.Locations
            .Select(l => new MapLocation
                {
                    Id = l.Id,
                    Name = l.Name,
                    Type = (short)l.Type,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude,
                    Address = l.Address,
                    Description = l.Description,
                    GooglePlacesId = l.GooglePlacesId
                }
            )
            .ToListAsync();
    }
}