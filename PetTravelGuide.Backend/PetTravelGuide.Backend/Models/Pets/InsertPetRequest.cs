namespace PetTravelGuide.Backend.Models.Pets;

public class InsertPetRequest
{
    public string Name { get; set; } = null!;

    public string Species { get; set; } = null!;
    
    public int Age { get; set; }
}