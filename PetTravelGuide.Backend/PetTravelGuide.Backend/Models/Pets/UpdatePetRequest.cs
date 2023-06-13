namespace PetTravelGuide.Backend.Models.Pets;

public class UpdatePetRequest : InsertPetRequest
{
    public long Id { get; set; }
}