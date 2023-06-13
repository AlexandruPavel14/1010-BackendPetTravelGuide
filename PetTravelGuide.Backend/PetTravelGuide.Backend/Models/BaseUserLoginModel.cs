namespace PetTravelGuide.Backend.Models;

/// <summary>
/// User data sent with a successful login request. Can be derived in client apps / other services
/// </summary>
public class BaseUserLoginModel
{
    /// <summary>
    /// The users first name
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// The users last name (family name)
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// The users email
    /// </summary>
    public string Email { get; set; } = null!;

    /*
    /// <summary>
    /// The member's picture / avatar URL
    /// </summary>
    public string? PictureUrl { get; set; }
    */
}