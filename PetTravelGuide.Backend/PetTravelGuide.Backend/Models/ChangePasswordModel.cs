using System.ComponentModel.DataAnnotations;

namespace PetTravelGuide.Backend.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public string OldPassword { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;
    }
}
