using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace Events.API.Repository
{
  public class ParticipantRepository : IParticipantRepository
  {
    private readonly DataContext _context;
    private readonly ILogger<ParticipantRepository> _logger;


    public ParticipantRepository(DataContext context, ILogger<ParticipantRepository> logger)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public async Task<ICollection<Participant>> GetParticipantsByEventIdAsync(Guid eventId, int page=1, int size=5)
    {
      try
      {
        if (eventId == Guid.Empty)
        {
          _logger.LogInformation("Invalid event ID provided.");
          return null;
        }

        if (page < 1 || size < 1|| size> 10)
        {
          _logger.LogInformation("Invalid pagination parameters provided.");
          page = 1;
          size = 5;
        }
        var participants = await _context.Participants
            .Where(p => p.EventId == eventId)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
        _logger.LogInformation(participants.Count == 0
          ? @$"No participants found for event with ID {eventId}."
          : @$"Retrieved {participants.Count} participants for event with ID {eventId}.");

        return participants;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, @$"An error occurred while retrieving participants for event with ID {eventId}.");
        return null;
      }
    }

    public async Task<Participant> GetParticipantByIdAsync(Guid id, Guid eventId)
    {
      try
      {
        if (id == Guid.Empty || eventId == Guid.Empty)
        {
          _logger.LogInformation("Invalid participant ID or event ID provided.");
          return null;
        }
        var participant = await _context.Participants
            .Where(p => p.Id == id && p.EventId == eventId)
            .FirstOrDefaultAsync();
        _logger.LogInformation(participant == null
          ? @$"Participant with ID {id} not found for event with ID {eventId}."
          : @$"Retrieved participant with ID {id} for event with ID {eventId}.");

        return participant;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, @$"An error occurred while retrieving participant with UserID {id} for event with ID {eventId}.");
        return null;
      }
    }

    public async Task RegisterParticipantAsync(Participant participant, Guid eventId)
    {
      try
      {
        if (eventId == Guid.Empty)
        {
          _logger.LogInformation("Invalid event ID provided.");
          return;
        }

        if (participant == null)
        {
          _logger.LogInformation("Participant object is null.");
          return;
        }
        var existingEvent = await _context.Events.FindAsync(eventId);
        if (existingEvent == null)
        {
          _logger.LogInformation(@$"Event with ID {eventId} not found.");
          return;
        }

        participant.EventId = eventId;
        await _context.Participants.AddAsync(participant);
        _logger.LogInformation(@$"Participant with UserID {participant.UserId} registered for event with ID {eventId}.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, @$"An error occurred while registering participant with UserID {participant.UserId} for event with ID {eventId}.");
        return;
      }

      await Task.CompletedTask;
    }

    public async Task RegisterParticipantsAsync(List<Participant> participants, Guid eventId)
    {
      try
      {
        if (eventId == Guid.Empty)
        {
          _logger.LogInformation("Invalid event ID provided.");
          return;
        }

        if (participants == null ||!participants.Any())
        {
          _logger.LogInformation("Participant list is is null or empty.");
          return;
        }
        var existingEvent = await _context.Events.FindAsync(eventId);
        if (existingEvent == null)
        {
          _logger.LogInformation(@$"Event with ID {eventId} not found.");
          return;
        }
        
        foreach (var participant in participants)
        {
          participant.EventId = eventId;
          await _context.Participants.AddAsync(participant);
          _logger.LogInformation(@$"Participant with UserID {participant.UserId} registered for event with ID {eventId}.");
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, @$"An error occurred while registering participants for event with ID {eventId}.");
        return;
      }

      await Task.CompletedTask;
    }

    public async Task RemoveParticipantAsync(Guid userId, Guid eventId)
    {
      try
      {
        if (userId == null)
        {
          _logger.LogInformation("Invalid UserId.");
          return;
        }

        if (eventId == Guid.Empty)
        {
          _logger.LogWarning("Invalid event ID provided.");
          return;
        }

        var existingParticipant = await _context.Participants
          .Where(p => p.UserId == userId && p.EventId == eventId)
          .FirstOrDefaultAsync();

        if (existingParticipant == null)
        {
          _logger.LogWarning(@$"Participant with UserID {userId} not found for event with ID {eventId}.");
          return;
        }
        _context.Participants.Remove(existingParticipant);
        _logger.LogInformation(@$"Participant with ID {userId} removed from event with ID {eventId}.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, @$"An error occurred while deleting participant for event with ID {eventId}.");
        return;
      }

      await Task.CompletedTask;

    }
  }
}
