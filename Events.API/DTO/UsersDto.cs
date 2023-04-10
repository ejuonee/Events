namespace Events.API.DTO;

public class UsersDto
{
  public int UserId { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string UserName { get; set; }
  public string Email { get; set; }
}

public class UsersExportDto
{
  public int UserId { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string UserName { get; set; }
  public string Email { get; set; }
  public ICollection<ExportEventDto> OwnedEvents { get; set; }
  [JsonPropertyName("RegisteredEvents")]
  public ICollection<ExportParticipantsDto> ParticipantEvents { get; set; }
  public ICollection<ExportInvitationDto> Invites { get; set; }
}

