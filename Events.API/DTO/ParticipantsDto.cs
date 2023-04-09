namespace Events.API.DTO;

public class ParticipantsDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}

public class RegisterParticipantsDto
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
}