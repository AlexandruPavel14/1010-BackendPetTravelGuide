namespace PetTravelGuide.Backend.Database.Entities;

public class Pet
{
    public long Id { get; set; }
    
    public long UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Species { get; set; } = null!;
    
    // This is measured in days because PostgreSQL intervals with month or year components cannot be read as TimeSpan.
    public TimeSpan Age { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}