public class Pet
{
    public int PetId { get; set; }
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Species { get; set; }
    public int Age { get; set; }
    public string? Color { get; set; }
    public DateTime AddedAt { get; set; }

    public User User { get; set; }
}