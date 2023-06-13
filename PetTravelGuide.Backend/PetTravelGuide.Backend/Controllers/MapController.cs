using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetTravelGuide.Backend.Models.Map;
using PetTravelGuide.Backend.Models.Response;
using PetTravelGuide.Backend.Services.MapService;
using PetTravelGuide.Backend.Utils;

namespace PetTravelGuide.Backend.Controllers;

[ApiController]
[Authorize(Roles = "Administrator, Reviewer, User")]
[Route("api/map")]
public class MapController : PtgController
{
    private readonly IMapService _mapService;

    public MapController(IMapService mapService)
    {
        _mapService = mapService;
    }

    [HttpGet("markers")]
    public async Task<IActionResult> Markers()
    {
        return Ok(
            new SuccessDataResponse<List<MapLocation>>(
                await _mapService.GetLocationMarkers()
            )
        );
    }
}