namespace PetTravelGuide.Backend.Database.Entities;

public partial class UsersRole
{
    public long UserId { get; set; }
    public short RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}