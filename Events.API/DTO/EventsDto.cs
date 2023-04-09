namespace Events.API.DTO;

public class EventsDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ICollection<ParticipantsDto> Participants { get; set; }
    public ICollection<InvitationsDto> Invites { get; set; }
}


public class RegisterEventDto
{
    public Guid OwnerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
}