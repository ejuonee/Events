namespace Events.API.Models
{
  public class Invitation
  {
    private Guid Id { get; set; }
    private Guid EventId { get; set; }
    private Guid InviterId { get; set; }
    private Guid InviteeId { get; set; }
    private InvitationStatus InviteState { get; set; }

    public Invitation(Guid eventId, Guid inviterId, Guid inviteeId)
    {
      Id = Guid.NewGuid();
      EventId = eventId;
      InviterId = inviterId;
      InviteeId = inviteeId;
      InviteState = InvitationStatus.Pending;
    }

    public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
    {
      public void Configure(EntityTypeBuilder<Invitation> builder)
      {
        builder.Property(i => i.Id).ValueGeneratedOnAdd();
        builder.Property(i => i.EventId);
        builder.Property(i => i.InviterId);
        builder.Property(i => i.InviteeId);
        builder.Property(i => i.InviteState)
            .HasConversion<string>();
      }
    }

    public Guid GetId()
    {
      return Id;
    }

    public Guid GetEventId()
    {
      return EventId;
    }

    public Guid GetInviterId()
    {
      return InviterId;
    }
    public Guid GetInviteeId()
    {
      return InviteeId;
    }
    public InvitationStatus GetInviteState()
    {
      return this.InviteState;
    }

    public void ChangeInviteState(InvitationStatus newState)
    {
      this.InviteState = newState;
    }
  }


}