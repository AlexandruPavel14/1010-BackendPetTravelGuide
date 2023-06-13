using System.ComponentModel.DataAnnotations;

namespace PetTravelGuide.Backend.Models.AppSettings;

public class JwtAuthenticationSettings
{
    [Required]
    public int ExpiryInMinutes { get; set; }

    [Required]
    public string Issuer { get; set; } = null!;

    [Required]
    public string Audience { get; set; } = null!;

    [Required]
    public string SigningKey { get; set; } = null!;
}