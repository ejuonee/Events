namespace Events.API.Interfaces;

public interface IParticipantRepository
{
  Task<ICollection<User>> GetParticipantsByEventIdAsync(Guid eventId);
  Task<User> GetParticipantByIdAsync(Guid id, Guid eventId);
  Task RegisterParticipantAsync(User participant, Guid eventId, Guid inviteeId);
  Task RegisterParticipantsAsync(List<User> participant, Guid eventId, Guid inviteeId);
  Task UpdateParticipantApprovalStatusAsync(Guid participantId, InvitationStatus status, Guid eventId, Guid organizerId);
  Task RemoveParticipantAsync(Guid id, Guid eventId, Guid organizerId);
}