using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Repository
{
  public class ParticipantRepository : IParticipantRepository
  {
    private readonly DataContext _context;
    private readonly ILogger<ParticipantRepository> _logger;


    public ParticipantRepository(DataContext context, ILogger<ParticipantRepository> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task<User> GetParticipantByIdAsync(Guid id, Guid eventId)
    {
      if (id == Guid.Empty || eventId == Guid.Empty)
      {
        _logger.LogWarning("Invalid participant ID or event ID provided.");
        return null;
      }

      var invitation = await _context.Invitations
          .Where(i => i.GetInviteeId() == id && i.GetEventId() == eventId)
          .FirstOrDefaultAsync();

      if (invitation.GetInviteeId() == null)
      {
        _logger.LogWarning($"Participant with ID {id} not found for event with ID {eventId}.");
        return null;
      }

      var participant = await _context.Users.FirstAsync(i => i.GetId() == invitation.GetInviteeId());
      _logger.LogInformation($"Retrieved participant with ID {id} for event with ID {eventId}.");
      return participant;

    }

    public async Task<ICollection<User>> GetParticipantsByEventIdAsync(Guid eventId)
    {
      if (eventId == Guid.Empty)
      {
        _logger.LogWarning("Invalid event ID provided.");
        return null;
      }

      ICollection<User> users = new List<User>();

      try
      {
        var invites = await _context.Invitations
            .Where(i => i.GetId() == eventId).ToListAsync();

        foreach (var invite in invites)
        {
          var user = await _context.Users.Where<User>(i => i.GetId() == invite.GetInviteeId()).FirstOrDefaultAsync();

          if (user != null)
          {
            users.Add(user);
          }

        }

      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while retrieving participants for event with ID {eventId}.");
        return null;
      }

      if (users.Count == 0)
      {
        _logger.LogWarning($"No participants found for event with ID {eventId}.");
      }
      else
      {
        _logger.LogInformation($"Retrieved {users.Count} participants for event with ID {eventId}.");
      }

      return users;
    }

    public async Task RegisterParticipantAsync(User participant, Guid eventId, Guid inviterId)
    {
      if (eventId == Guid.Empty)
      {
        _logger.LogWarning("Invalid event ID provided.");
        return;
      }
      var existingEvent = await _context.Events.FindAsync(eventId);
      if (existingEvent == null)
      {
        _logger.LogWarning($"Event with ID {eventId} not found.");
        return;
      }
      var invitation = new Invitation(eventId, inviterId, participant.GetId(), InvitationStatus.Accepted);
      _context.Invitations.Add(invitation);
      _context.Entry(invitation).State = EntityState.Added;
      _logger.LogInformation($"Participant with ID {participant.GetId()} registered for event with ID {eventId}.");

    }

    public async Task RegisterParticipantsAsync(List<User> participants, Guid eventId, Guid inviterId)
    {
      if (eventId == Guid.Empty)
      {
        _logger.LogWarning("Invalid event ID provided.");
        return;
      }
      var existingEvent = await _context.Events.FindAsync(eventId);
      if (existingEvent == null)
      {
        _logger.LogWarning($"Event with ID {eventId} not found.");
        return;
      }
      foreach (var user in participants)
      {
        var invitation = new Invitation(eventId, inviterId, user.GetId(), InvitationStatus.Accepted);
        _context.Invitations.Add(invitation);
        _context.Entry(invitation).State = EntityState.Added;
        _logger.LogInformation($"Participant with ID {user.GetId()} registered for event with ID {eventId}.");
      }



    }

    public async Task RemoveParticipantAsync(Guid id, Guid eventId, Guid organizerId)
    {
      var existingParticipant = await _context.Users.FindAsync(id);

      if (existingParticipant == null)
      {
        _logger.LogWarning($"Participant with ID {id} not found.");
        return;
      }

      var eventToRemoveFrom = await _context.Events.FindAsync(eventId);

      if (eventToRemoveFrom == null)
      {
        _logger.LogWarning($"Event with ID {eventId} not found.");
        return;
      }

      if (eventToRemoveFrom.GetOwnerId() != organizerId)
      {
        _logger.LogWarning($"User with ID {organizerId} is not authorized to remove participant with ID {id}.");
        return;
      }

      var invitationsToRemove = await _context.Invitations
          .Where(i => i.GetInviteeId() == id && i.GetEventId() == eventId)
          .ToListAsync();

      foreach (var invitation in invitationsToRemove)
      {
        _context.Invitations.Remove(invitation);
      }

      _logger.LogInformation($"Participant with ID {id} removed from event with ID {eventId}.");

    }

    public async Task UpdateParticipantApprovalStatusAsync(Guid participantId, InvitationStatus status, Guid eventId, Guid organizerId)
    {
      var existingParticipant = await _context.Users.FindAsync(participantId);
      if (existingParticipant == null)
      {
        _logger.LogWarning($"Participant with ID {participantId} not found.");
        return;
      }
      var existingInvitation = await _context.Invitations.Where(i => i.GetInviteeId() == existingParticipant.GetId() && i.GetEventId() == eventId).FirstOrDefaultAsync();
      if (existingInvitation == null)
      {
        _logger.LogWarning($"Participant with ID {participantId} was not invited to this event.");
        return;
      }
      if (existingInvitation.GetInviterId() != organizerId)
      {
        _logger.LogWarning($"User with ID {organizerId} is not authorized to update participant with ID {participantId}.");
        return;
      }
      existingInvitation.ChangeInviteState(status);
      _logger.LogInformation($"Participant with ID {participantId} updated approval status to {status}.");
    }
  }
}
