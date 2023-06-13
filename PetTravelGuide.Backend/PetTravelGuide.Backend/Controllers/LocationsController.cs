using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetTravelGuide.Backend.Database;
using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models.Locations;
using PetTravelGuide.Backend.Models.Response;
using PetTravelGuide.Backend.Services.LocationService;
using PetTravelGuide.Backend.Utils;

namespace PetTravelGuide.Backend.Controllers;

[ApiController]
[Authorize(Roles = "Administrator, Reviewer, User")]
[Route("api/locations")]
public class LocationsController : PtgController
{
    private readonly ILocationService _locationService;
    
    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }
    
    [HttpGet("my")]
    public async Task<IActionResult> MyLocations()
    {
        // Get user Id from JWT
        if (RequestUserId is null)
        {
            return BadRequest("Invalid user id for the request");
        }
        
        return Ok(
            new SuccessDataResponse<List<Location>>(
                await _locationService.GetAll(RequestUserId.Value)
            )
        );
    }

    [HttpGet("all")]
    public async Task<IActionResult> AllLocations()
    {
        return Ok(
            new SuccessDataResponse<List<Location>>(
                await _locationService.GetAll()
            )
        );
    }
    
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] InsertLocationRequest request)
    {
        if (RequestUserId is null)
        {
            return BadRequest("Invalid user id for the request");
        }

        Location location = new()
        {
            Name = request.Name,
            Description = request.Description,
            Type = (PlaceType)request.Type,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            GooglePlacesId = request.GooglePlacesId,
            UserId = RequestUserId.Value
        };
        
        IDataResponse<Location> response = await _locationService.Insert(location, RequestUserId.Value);
        if (!response.IsSuccess)
        {
            return Ok(new ErrorDataResponse<Location>(null!, response.Message!));
        }
        
        return Ok(response);
    }
    
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] Location request)
    {
        if (RequestUserId is null)
        {
            return BadRequest("Invalid user id for the request");
        }

        if (RequestUserRoles is null)
        {
            return BadRequest("Invalid user role for the request");
        }
        
        long? userId = null;
        // Administrator and Reviewers can update locations
        if (RequestUserRoles == "User")
        {
            userId = RequestUserId.Value;
        }

        IDataResponse<Location> response = await _locationService.Update(request, userId);
        if (!response.IsSuccess)
        {
            return Ok(new ErrorDataResponse<Location>(null!, response.Message!));
        }

        return Ok(response);
    }
    
    [HttpGet("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (RequestUserId is null)
        {
            return BadRequest("Invalid user id for the request");
        }

        if (RequestUserRoles is null)
        {
            return BadRequest("Invalid user role for the request");
        }
        
        long? userId = null;
        // Administrator and Reviewers can delete every location
        // Users can delete only their location
        if (RequestUserRoles == "User")
        {
            userId = RequestUserId.Value;
        }

        return Ok(await _locationService.DeleteById(id, userId));
    }
}