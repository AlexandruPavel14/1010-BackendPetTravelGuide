using PetTravelGuide.Backend.Models.Map;

namespace PetTravelGuide.Backend.Services.MapService;

public interface IMapService
{
    Task<List<MapLocation>> GetLocationMarkers();
}