using System.Collections;

namespace PetTravelGuide.Backend.Database.Entities;

public class User
{
    public long Id { get; init; }

    public string Email { get; set; } = null!;
    
    // public bool EmailConfirmed { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
    
    public char? Gender { get; set; }
    
    public string PasswordHash { get; set; } = null!;
    
    public string? PhoneNumber { get; set; }
    
    // public DateTime? LockoutEnd { get; set; }
    
    // public bool LockoutEnabled { get; set; }
    
    // public int AccessFailedCount { get; set; }
    
    // public string? PictureUrl { get; set; }
    
    public DateTime CreatedAt { get; init; }
    
    public DateTime? ModifiedAt { get; init; }
    
    public bool? Active { get; init; }
    
    public virtual ICollection<Role> Roles { get; set; }
    
    public virtual ICollection<Pet> Pets { get; set; }
    
    // public virtual ICollection<Location> Locations { get; set; }

    public User()
    {
        Roles = new HashSet<Role>();
        Pets = new HashSet<Pet>();
        // Locations = new HashSet<Location>();
    }
}