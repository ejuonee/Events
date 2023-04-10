using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Repository
{
  public class InvitationRespository : IInvitationRespository
  {
    private readonly DataContext _context;
    private readonly ILogger<InvitationRespository> _logger;


    public InvitationRespository(DataContext context, ILogger<InvitationRespository> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task CreateInvitationAsync(Invitation invitation)
    {
      if (invitation == null)
      {
        _logger.LogError("Invitation cannot be null");
        throw new ArgumentNullException(nameof(invitation));
      }
      // Check if the event and user exist
      var @event = await _context.Events.FindAsync(invitation.EventId);
      var user = await _context.Users.FindAsync(invitation.InvitedId);


      if (@event == null)
      {
        _logger.LogInformation("Event or does not exist.");
        throw new KeyNotFoundException(@$"Event with id {invitation.EventId} does not exist");
      }

      if (user == null)
      {
        _logger.LogInformation("User or does not exist.");
        throw new KeyNotFoundException(@$"User with id {invitation.InvitedId} does not exist");
      }

      var eventOwner = await _context.Users.FindAsync(@event.OwnerId);
      invitation.InviterUserName = eventOwner.UserName;
      await _context.Invitations.AddAsync(invitation);
      _logger.LogInformation(@$"Created new invitation with ID {invitation.InvitationId}.");

      await Task.CompletedTask;
    }

    public async Task DeleteInvitationAsync(int id)
    {

      if (!Int32.IsPositive(id))
      {
        _logger.LogError("Invalid invitation ID provided.");
        throw new ArgumentException("Invalid invitation ID provided.");
      }

      var invitation = await _context.Invitations.FindAsync(id);
      if (invitation == null)
      {
        _logger.LogError(@$"Invitation with ID {id} not found.");
        throw new KeyNotFoundException(@$"Invitation with ID {id} not found.");
      }
      _context.Invitations.Remove(invitation);
      _logger.LogInformation(@$"Deleted invitation with ID {id}.");

      await Task.CompletedTask;
    }

    public async Task<Invitation> GetInvitationByInvitationIdandEventIdAsync(int invitationId, int eventId)
    {
      if (!Int32.IsPositive(invitationId))
      {
        _logger.LogError("Invalid invitation ID provided.");
        throw new ArgumentException("Invalid invitation ID provided.");
      }
      var invitation = await _context.Invitations.Where(x => x.InvitationId == invitationId && x.EventId == eventId).FirstOrDefaultAsync();
      if (invitation == null)
      {
        _logger.LogError(@$"Invitation with ID {invitationId} not found for event {eventId}.");
        throw new KeyNotFoundException(@$"Invitation with ID {invitationId} not found for event {eventId}.");
      }

      _logger.LogInformation(@$"Retrieved invitation with ID {invitationId}.");
      return invitation;
    }

    public async Task<ICollection<Invitation>> GetInvitationsByEventIdAsync(int eventId, int page = 1, int size = 5)
    {
      if (!Int32.IsPositive(eventId))
      {
        _logger.LogError("Invalid invitation ID provided.");
        throw new ArgumentException("Invalid event ID provided.");

      }
      if (page < 1 || size < 1 || size > 10)
      {
        _logger.LogInformation("Invalid pagination parameters provided.");
        page = 1;
        size = 10;
      }
      var invitations = await _context.Invitations
          .Where(i => i.EventId == eventId)
          .Skip((page - 1) * size)
          .Take(size)
          .ToListAsync();
      _logger.LogInformation(!invitations.Any()
        ? @$"No invitations found for event with ID {eventId}."
        : @$"Retrieved {invitations.Count()} invitations for event with ID {eventId}.");

      return invitations;

    }
    public async Task UpdateInvitationAsync(Invitation invitation)
    {
      if (invitation == null)
      {
        _logger.LogError("Invitation cannot be null");
        throw new ArgumentNullException(nameof(invitation), "Invitation cannot be null");
      }

      var invitations = await _context.Invitations.FindAsync(invitation.InvitationId);
      if (invitations == null)
      {
        _logger.LogError(@$"Invitation not found for ID: {invitation.InvitationId}");
        throw new KeyNotFoundException(@$"Invitation not found for ID: {invitation.InvitationId}");
      }
      _context.Entry(invitations).State = EntityState.Modified;
      _logger.LogInformation($"Updated invitation with ID {invitation.InvitationId}.");

      await Task.CompletedTask;
    }
  }
}
