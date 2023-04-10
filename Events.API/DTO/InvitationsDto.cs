namespace Events.API.DTO;

public class InvitationsDto
{
  public int InvitationId { get; set; }
  public int EventId { get; set; }
  public int InvitedId { get; set; }
  public string InvitationMessage { get; set; }
  public string InviterUserName { get; set; }
  public InvitationStatus InviteState { get; set; }
}

public class CreateInvitationDto
{
  public int InvitedId { get; set; }
  public string InvitationMessage { get; set; }
  public InvitationStatus InviteState = InvitationStatus.Pending;
}
public class UpdateInvitation
{
  public InvitationStatus InviteState { get; set; }
}
public class ExportInvitationDto
{
  public int InvitationId { get; set; }
  public int EventId { get; set; }
  public int InvitedId { get; set; }
  public string InvitationMessage { get; set; }
  public string InviterUserName { get; set; }
  public InvitationStatus InviteState { get; set; }
}