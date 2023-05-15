public class Invitation
{
    public int InvitationId { get; set; }
    public string? InvitationCode { get; set; }

    public int? CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    public ICollection<UserInvitation> UserInvitations { get; set; }
}
