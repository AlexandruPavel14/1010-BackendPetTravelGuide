namespace PetTravelGuide.Backend.Models;

public class ProfileResponseModel : BaseUserLoginModel
{
    /// <summary>
    /// The users phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
}
