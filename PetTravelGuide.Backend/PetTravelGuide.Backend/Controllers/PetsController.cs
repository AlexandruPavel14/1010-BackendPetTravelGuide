using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models.Pets;
using PetTravelGuide.Backend.Models.Response;
using PetTravelGuide.Backend.Services.PetService;
using PetTravelGuide.Backend.Utils;

namespace PetTravelGuide.Backend.Controllers;

[ApiController]
[Authorize(Roles = "Administrator, Reviewer, User")]
[Route("api/pets")]
public class PetsController : PtgController
{
    private readonly IPetService _petService;
    
    public PetsController(IPetService petService)
    {
        _petService = petService;
    }

    [HttpGet]
    public async Task<IActionResult> Pets()
    {
        // Get user Id from JWT
        if (RequestUserId is null)
        {
            return BadRequest("Invalid user id for the request");
        }

        return Ok(
            new SuccessDataResponse<List<UpdatePetRequest>>(
                (await _petService.GetAll(RequestUserId.Value)).Select(p => new UpdatePetRequest
                {
                    Id = p.Id,
                    Name = p.Name,
                    Species = p.Species,
                    // Convert interval to days
                    Age = Convert.ToInt32(p.Age.TotalDays)
                }).ToList()
            )
        );
    }

    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] InsertPetRequest request)
    {
        // Get user Id from JWT
        if (RequestUserId is null)
        {
            return BadRequest("Invalid user id for the request");
        }

        Pet pet = new()
        {
            Name = request.Name,
            Species = request.Species,
            // Convert from integer to TimeSpan
            Age = TimeSpan.FromDays(request.Age)
        };

        IDataResponse<Pet> response = await _petService.Insert(pet, RequestUserId.Value);
        if (!response.IsSuccess)
        {
            return Ok(new ErrorDataResponse<Pet>(null!, response.Message!));
        }

        return Ok(
            new SuccessDataResponse<InsertPetResponse>(
                new InsertPetResponse
                {
                    Id = response.Data.Id,
                    Name = response.Data.Name,
                    Species = response.Data.Species,
                    Age = Convert.ToInt32(response.Data.Age.TotalDays)
                },
                response.Message!
            )
        );
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdatePetRequest request)
    {
        // Get user Id from JWT
        if (RequestUserId is null)
        {
            return BadRequest("Invalid user id for the request");
        }

        Pet pet = new()
        {
            Id = request.Id,
            Name = request.Name,
            Species = request.Species,
            // Convert from integer to TimeSpan
            Age = TimeSpan.FromDays(request.Age),
            UserId = RequestUserId.Value
        };
        
        IDataResponse<Pet> response = await _petService.Update(pet, RequestUserId.Value);
        if (!response.IsSuccess)
        {
            return Ok(new ErrorDataResponse<Pet>(null!, response.Message!));
        }
        
        return Ok(
            new SuccessDataResponse<UpdatePetResponse>(
                new UpdatePetResponse
                {
                    Id = response.Data.Id,
                    Name = response.Data.Name,
                    Species = response.Data.Species,
                    // Convert from TimeSpan to integer
                    Age = Convert.ToInt32(response.Data.Age.TotalDays)
                },
                response.Message!
            )
        );
    }

    [HttpGet("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        // Get user Id from JWT
        if (RequestUserId is null)
        {
            return BadRequest("Invalid user id for the request");
        }

        return Ok(await _petService.DeleteById(id, RequestUserId.Value));
    }
}