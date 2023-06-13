using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models;
using PetTravelGuide.Backend.Models.AppSettings;
using PetTravelGuide.Backend.Models.Response;
using PetTravelGuide.Backend.Services.UserService;

namespace PetTravelGuide.Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly JwtAuthenticationSettings _jwtOptions;
    private readonly UserServiceSettings _userServiceSettings;
    
    public AuthController(
        IUserService userService,
        IOptions<JwtAuthenticationSettings> jwtOptions,
        IOptions<UserServiceSettings> userServiceSettings)
    {
        _userService = userService;
        _jwtOptions = jwtOptions.Value;
        _userServiceSettings = userServiceSettings.Value;
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestModel userLoginInfo)
    {
        if (userLoginInfo == null)
            throw new Exception($"ParameterNullOrNotSet: {nameof(userLoginInfo)}");
        IResponse result = await _userService.Login(userLoginInfo.Email, userLoginInfo.Password);
        return Ok(!result.IsSuccess ? new ErrorResponse(result.Message) : result);
    }
    
    /// <summary>
    /// Register user action. Will be used only by the Members app
    /// </summary>
    /// <param name="userRegisterInfo">The UserRegisterRequestModel</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequestModel userRegisterInfo)
    {
        if (userRegisterInfo == null)
            throw new Exception($"ParameterNullOrNotSet: {nameof(userRegisterInfo)}");
        // Create user entity
        User user = new()
        {
            Email = userRegisterInfo.Email,
            FirstName = userRegisterInfo.FirstName,
            PhoneNumber = userRegisterInfo.PhoneNumber,
            LastName = userRegisterInfo.LastName,
            Active = true
        };
        return Ok(await _userService.InsertUser(user, userRegisterInfo.Password, userRegisterInfo.IsReviewer));
    }
}