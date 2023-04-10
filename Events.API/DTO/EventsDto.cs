namespace Events.API.DTO;

public class EventsDto
{
  public int EventId { get; set; }
  public int OwnerId { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public string Location { get; set; }
    
  public EventType EventType { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public ICollection<ParticipantsDto> Participants { get; set; }
  public ICollection<InvitationsDto> Invites { get; set; }
}


public class RegisterEventDto
{
  public int OwnerId { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public string Location { get; set; }
    
  public EventType EventType { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }

}

public class ExportEventDto
{
  public int EventId { get; set; }
  public int OwnerId { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public string Location { get; set; }
    
  public EventType EventType { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }

}

public class UpdateEventDto
{

  public string Title { get; set; }
  public string Description { get; set; }
  public string Location { get; set; }
    
  public EventType EventType { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }

  

}