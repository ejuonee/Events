namespace Events.API.Models
{
  public class Invitation
  {
    public Invitation(int eventId, int invitedId, InvitationStatus inviteState, string invitationMessage, string inviterUserName)
    {
      EventId = eventId;
      InvitedId = invitedId;
      InviteState = inviteState;
      InvitationMessage = invitationMessage;
      InviterUserName = inviterUserName;
    }

    public Invitation()
    {
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InvitationId { get; set; }
    public int EventId { get; set; }
    public int InvitedId { get; set; }
    public string InvitationMessage { get; set; }
    public string InviterUserName { get; set; }
    public InvitationStatus InviteState { get; set; }
  }


}