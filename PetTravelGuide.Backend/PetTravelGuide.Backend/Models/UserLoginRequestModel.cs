using System.ComponentModel.DataAnnotations;

namespace PetTravelGuide.Backend.Models
{
    /// <summary>
    /// The user login request model received from a client app that wants to authenticate an user
    /// </summary>
    public class UserLoginRequestModel
    {
        /// <summary>
        /// The users email
        /// </summary>
        [Required]
        public string Email { get; set; } = null!;

        /// <summary>
        /// The users password
        /// </summary>
        [Required]
        public string Password { get; set; } = null!;
    }
}
