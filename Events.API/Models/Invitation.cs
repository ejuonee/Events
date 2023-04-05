namespace Events.API.Models
{
  public class Invitation
  {
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Event Event { get; set; }

    public Guid EventOwnerId { get; set; }
    public User EventOwner { get; set; }

    public Guid InvitedId { get; set; }
    public User Invited { get; set; }
    public InvitationStatus InviteState { get; set; }

    public Invitation()
    {
      Id = Guid.NewGuid();
      InviteState = InvitationStatus.Pending;
    }
  }


}