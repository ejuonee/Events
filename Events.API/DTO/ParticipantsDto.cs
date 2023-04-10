namespace Events.API.DTO;

public class ParticipantsDto
{
  public int ParticipantId { get; set; }
  public int UserId { get; set; }
  public int EventId { get; set; }
}

public class RegisterParticipantsDto
{
  public int UserId { get; set; }
}
public class ExportParticipantsDto
{
  public int ParticpantId { get; set; }
  public int UserId { get; set; }
  public int EventId { get; set; }
}