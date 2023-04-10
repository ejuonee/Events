namespace Events.API.Interfaces;

public interface IParticipantRepository
{
  Task<ICollection<Participant>> GetParticipantsByEventIdAsync(int eventId, int page, int size);
  Task<Participant> GetParticipantByIdAsync(int participantId, int eventId);
  Task RegisterParticipantAsync(Participant participant);
  Task<List<Participant>> RegisterParticipantsAsync(List<Participant> participant);
  Task RemoveParticipantAsync(int participantId, int eventId);
  Task<Participant>GetParticipantByEventAndUserId(int eventId, int userId);
}