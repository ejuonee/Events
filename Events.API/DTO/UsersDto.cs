namespace Events.API.DTO;

public class UsersDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}

public class UsersExportDto
{

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public ICollection<ExportEventDto> OwnedEvents { get; set; }
    public ICollection<ExportParticipantsDto> ParticipantEvents { get; set; }
    public ICollection<ExportInvitationDto> Invites { get; set; }
}