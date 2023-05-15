public class User
{
    public int UserId { get; set; }
    public string? Nume { get; set; }
    public string? Prenume { get; set; }
    public string? NrTelefon { get; set; }
    public string? Email { get; set; }
    public string? Parola { get; set; }

    public bool IsAdmin { get; set; } = false; // Default to false
    public bool IsModerator { get; set; } = false; // Default to false
    public bool IsUser { get; set; } = false; // Default to false

    public string? PasswordResetCode { get; set; }
    public DateTime? PasswordResetCodeExpirationDate { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }
    public List<Pet> Pets { get; set; }
    public ICollection<UserInvitation> UserInvitations { get; set; }

}



