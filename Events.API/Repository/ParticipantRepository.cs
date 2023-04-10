using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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


    public async Task<ICollection<Participant>> GetParticipantsByEventIdAsync(int eventId, int page = 1, int size = 5)
    {
      if (!Int32.IsPositive(eventId))
      {
        _logger.LogError("Invalid event ID provided.");
        throw new ArgumentException("Invalid event ID provided.");
      }

      if (page < 1 || size < 1 || size > 10)
      {
        _logger.LogError("Invalid pagination parameters provided.");
        page = 1;
        size = 10;
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

    public async Task<Participant> GetParticipantByIdAsync(int particpantId, int eventId)
    {

        if (!Int32.IsPositive(particpantId))
        {
          _logger.LogError("Invalid ParticipantId ID provided.");
          throw new ArgumentException("Invalid ParticipantId ID provided.");

        }
        if (!Int32.IsPositive(eventId))
        {
          _logger.LogInformation("Invalid Event ID Provided.");
          throw new ArgumentException("Invalid Event ID Provided");
        }
        _logger.LogInformation(@$"Getting event with Participant with ID {particpantId} and Event ID {eventId} from the database");
        var participant = await _context.Participants
            .Where(p => p.ParticipantId == particpantId && p.EventId == eventId)
            .FirstOrDefaultAsync();
        if (participant == null)
        {
          _logger.LogError($"Participant with ID {particpantId} does not exist for this Event {eventId}.");
          throw new KeyNotFoundException(@$"Participant with ID {particpantId} not found in this Event {eventId}.");
        }
        

        _logger.LogInformation($"Successfully retrieved participant with Event ID {eventId} and Participant ID {particpantId} from the database.");
        return participant;
    }

    public async Task RegisterParticipantAsync(Participant participant)
    {
      if (participant == null)
        {
          _logger.LogInformation("Participant object is null.");
          throw new ArgumentException("Invalid Participant Object provided.");
        }
      var @event = await _context.Events.FindAsync(participant.EventId);
      var user = await _context.Users.FindAsync(participant.UserId);


      if (@event == null)
      {
        _logger.LogInformation("Event or does not exist.");
        throw new KeyNotFoundException(@$"Event with id {participant.EventId} does not exist");
      }

      if (user == null)
      {
        _logger.LogInformation("User or does not exist.");
        throw new KeyNotFoundException(@$"User with id {participant.UserId} does not exist");
      }
      // Check if participant already exists
      var existingParticipant = await _context.Participants.SingleOrDefaultAsync(p => p.UserId == participant.UserId && p.EventId == participant.EventId);

        if (existingParticipant != null)
        {
          _logger.LogInformation(@$"Participant with UserID {participant.UserId} already registered for event with ID {participant.EventId}.");
          throw new InvalidOperationException(@$"Participant with UserID {participant.UserId} already registered for event with ID {participant.EventId}.");
        }
        await _context.Participants.AddAsync(participant);
        _logger.LogInformation(@$"Participant with UserID {participant.UserId} registered for event with ID {participant.EventId}.");
        await Task.CompletedTask;
    }

    public async Task<List<Participant>> RegisterParticipantsAsync(List<Participant> participants)
    {
      if (participants == null)
      {
        _logger.LogInformation("Participants object is null.");
        throw new ArgumentException("Invalid Participant sObject provided.");
      }

      var failedParticipants = new List<Participant>();
      foreach (var participant in participants)
      {
        try
        {
          var @event = await _context.Events.FindAsync(participant.EventId);
          var user = await _context.Users.FindAsync(participant.UserId);


          if (@event == null)
          {
            _logger.LogInformation("Event or does not exist.");
            
          }
          else if (user == null)
          {
            _logger.LogInformation("User or does not exist.");
            failedParticipants.Add(participant);
          }
          else 
          {var existingParticipant = await _context.Participants.SingleOrDefaultAsync(p => p.UserId == participant.UserId && p.EventId == participant.EventId);

            if (existingParticipant != null)
            {
              _logger.LogInformation($"Participant with UserID {participant.UserId} already registered for event with ID {participant.EventId}.");
              failedParticipants.Add(participant);
            }
            else
            {
              await _context.Participants.AddAsync(participant);
            }
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, $"Failed to register participant with UserID {participant.UserId} for event with ID {participant.EventId}");
          
        }
      }
      await Task.CompletedTask;
      return failedParticipants;
    }

    public async Task RemoveParticipantAsync(int particpantId, int eventId)
    {
      if (!Int32.IsPositive(eventId))
      {
        _logger.LogError("Invalid Event ID provided.");
        throw new ArgumentException("Invalid Event ID provided.");
      }
      if (!Int32.IsPositive(particpantId))
      {
        _logger.LogError("Invalid participant ID provided.");
        throw new ArgumentException("Invalid participant ID provided.");
      }

      var existingParticipant = await _context.Participants
          .Where(p => p.ParticipantId == particpantId && p.EventId == eventId)
          .FirstOrDefaultAsync();

      if (existingParticipant == null)
      {
        _logger.LogError($"Participant with ID {particpantId} does not exist for this Event {eventId}.");
        throw new KeyNotFoundException(@$"Participant with ID {particpantId} not found in this Event {eventId}.");
      }
        _context.Participants.Remove(existingParticipant);
        _logger.LogInformation(@$"Participant with ID {particpantId} removed from event with ID {eventId}.");

        await Task.CompletedTask;

    }

    public async Task<Participant> GetParticipantByEventAndUserId(int eventId, int userId)
    {
      if (!Int32.IsPositive(eventId))
      {
        _logger.LogError("Invalid Event ID provided.");
        throw new ArgumentException("Invalid Event ID provided.");
      }
      if (!Int32.IsPositive(userId))
      {
        _logger.LogError("Invalid User ID provided.");
        throw new ArgumentException("Invalid USer ID provided.");
      }

      _logger.LogInformation($"Retrieving participant with Event ID {eventId} and User ID {userId} from the database.");
      var participant = await _context.Participants
        .Where(p => p.EventId == eventId && p.UserId == userId)
        .FirstOrDefaultAsync();

      if (participant == null)
      {
        _logger.LogInformation($"User with ID {userId} does not exist for this Event {eventId}.");
        return null;
      }
      _logger.LogInformation($"Successfully retrieved participant with Event ID {eventId} and User ID {userId} from the database.");


      return participant;
    }
  }
}
