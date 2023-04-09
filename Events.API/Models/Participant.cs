namespace Events.API.Models;

public class Participant
{
    public Participant( Guid userId, Guid eventId)
    {
        UserId = userId;
        EventId = eventId;
    }

    public Participant()
    {
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}