public class UserInvitation
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int InvitationId { get; set; }
    public Invitation Invitation { get; set; }
}