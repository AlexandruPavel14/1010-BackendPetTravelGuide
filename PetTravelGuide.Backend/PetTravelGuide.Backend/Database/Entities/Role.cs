namespace PetTravelGuide.Backend.Database.Entities;

public class Role
{
    public short Id { get; init; } 
    
    public string Name { get; set; } = null!;
    
    public string NormalizedName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; }

    public Role()
    {
        Users = new HashSet<User>();
    }
}