namespace Events.API.DTO;

public class InvitationsDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid InvitedId { get; set; }
    public InvitationStatus InviteState { get; set; }
}

public class CreateInvitationDto
{
    public Guid EventId { get; set; }
    public Guid InvitedId { get; set; }
    public InvitationStatus InviteState = InvitationStatus.Pending;
}