using System.ComponentModel.DataAnnotations;

namespace PetTravelGuide.Backend.Models.AppSettings;

public class UserServiceSettings
{
    // [Required]
    // public int AttemptsBeforeLockup { get; set; }

    // [Required]
    // public int LockupDurationMinutes { get; set; }

    [Required]
    public int PasswordMinLength { get; set; }

    [Required]
    public int PasswordMaxLength { get; set; }

    [Required]
    public int NameMinLength { get; set; }

    [Required]
    public int NameMaxLength { get; set; }
}
