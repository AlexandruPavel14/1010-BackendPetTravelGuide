namespace PetTravelGuide.Backend.Models;

/// <summary>
/// The response model sent back to a client app that wants to authenticate an user
/// </summary>
public class UserLoginResponseModel
{
    /// <summary>
    /// The users basic data sent back to the client app
    /// </summary>
    public BaseUserLoginModel UserInfo { get; set; } = null!;

    /// <summary>
    /// The JWT token generated when the credentials are validated
    /// </summary>
    public string Token { get; set; } = null!;
}
