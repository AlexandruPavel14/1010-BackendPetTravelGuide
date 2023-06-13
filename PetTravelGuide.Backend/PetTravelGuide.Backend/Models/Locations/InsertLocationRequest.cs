namespace PetTravelGuide.Backend.Models.Locations;

public class InsertLocationRequest
{
    public long? Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public short Type { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public string? Address { get; set; }
    
    public string? GooglePlacesId { get; set; }
    
    public long? UserId { get; set; }
}