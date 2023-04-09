namespace Events.API.Models
{
  public class Invitation
  {
    public Invitation(Guid eventId, Guid invitedId, InvitationStatus inviteState)
    {
      EventId = eventId;
      InvitedId = invitedId;
      InviteState = inviteState;
    }

    public Invitation()
    {
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid InvitedId { get; set; }
    public InvitationStatus InviteState { get; set; }
  }


}