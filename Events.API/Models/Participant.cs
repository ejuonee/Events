namespace Events.API.Models;

public class Participant
{
  public Participant(int userId, int eventId)
  {
    UserId = userId;
    EventId = eventId;
  }

  public Participant()
  {
  }

  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int ParticipantId { get; set; }
  public int UserId { get; set; }
  public int EventId { get; set; }
}