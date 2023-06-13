namespace PetTravelGuide.Backend.Database.Entities;

public class Location
{
    public long Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public PlaceType Type { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public string? Address { get; set; }
    
    public string? GooglePlacesId { get; set; }
    
    public long UserId { get; set; }
    
    public virtual User User { get; set; } = null!;
}