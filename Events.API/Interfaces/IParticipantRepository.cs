namespace Events.API.Interfaces;

public interface IParticipantRepository
{
  Task<ICollection<Participant>> GetParticipantsByEventIdAsync(Guid eventId, int page, int size);
  Task<Participant> GetParticipantByIdAsync(Guid userId, Guid eventId);
  Task RegisterParticipantAsync(Participant participant, Guid eventId);
  Task RegisterParticipantsAsync(List<Participant> participant, Guid eventId);
  Task RemoveParticipantAsync(Guid userId, Guid eventId);
}