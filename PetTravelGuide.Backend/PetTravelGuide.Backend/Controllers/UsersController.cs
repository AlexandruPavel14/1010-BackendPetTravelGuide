using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models;
using PetTravelGuide.Backend.Models.Response;
using PetTravelGuide.Backend.Services.UserService;
using PetTravelGuide.Backend.Utils;

namespace PetTravelGuide.Backend.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : PtgController 
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Administrator, Reviewer, User")]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        // Get user email from JWT
        if (RequestUserEmail is null)
        {
            return BadRequest("Invalid user email for the request");
        }

        User? user = await _userService.GetUserByEmail(RequestUserEmail);
        if (user is null)
        {
            return Ok(new ErrorResponse("Nu s-a găsit un utilizator aferent datelor din request"));
        }

        return Ok(
            new SuccessDataResponse<ProfileResponseModel>(
                new ProfileResponseModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email
                }
            )
        );
    }
    
    /// <summary>
    /// Change user password on demand
    /// </summary>
    /// <param name="model">The model with the necessary parameters</param>
    [Authorize(Roles = "Administrator,Reviewer,User")]
    [Route("change-password")]
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        // Get user email from JWT
        if (RequestUserEmail is null)
        {
            return BadRequest("Invalid user email for the request");
        }

        IResponse result = await _userService.ChangePasswordByEmail(RequestUserEmail, model.OldPassword, model.NewPassword);
        
        if (!result.IsSuccess)
        {
            return Ok(result);
        }

        return Ok(new SuccessResponse("Parola a fost schimbata"));
    }
}