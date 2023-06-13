using System.ComponentModel.DataAnnotations;

namespace PetTravelGuide.Backend.Models
{
    public class UserRegisterRequestModel
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }
        
        [Required]
        public string Password { get; set; } = null!;
        
        [Required]
        public bool IsReviewer { get; set; }
    }
}
