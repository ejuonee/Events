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
      try
      {
        if (invitation == null )
        {
          _logger.LogInformation("Invalid Invitation Details");
          return;
        }
        // Check if the event and user exist
        var @event = await _context.Events.FindAsync(invitation.EventId);
        var user = await _context.Users.FindAsync(invitation.InvitedId);

        if (@event == null || user == null)
        {
          _logger.LogInformation("Event or user does not exist.");
          return;
        }
        await _context.Invitations.AddAsync(invitation);
        _logger.LogInformation(@$"Created new invitation with ID {invitation.Id}.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An error occurred while creating a new invitation.");
        return;
      }
      await Task.CompletedTask;
    }

    public async Task DeleteInvitationAsync(Guid id)
    {
      try
      {
        if (id == Guid.Empty)
        {
          _logger.LogWarning("Invalid invitation ID provided.");
          return;
        }

        var invitation = await _context.Invitations.FindAsync(id);
        if (invitation == null)
        {
          _logger.LogWarning(@$"Invitation with ID {id} not found.");
          return;
        } 
        _context.Invitations.Remove(invitation);
        _logger.LogInformation(@$"Deleted invitation with ID {id}.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, @$"An error occurred while deleting invitation with ID {id}.");
        return;
      }
      await Task.CompletedTask;
    }

    public async Task<Invitation> GetInvitationByIdAsync(Guid id)
    {

      try
      {
        if (id == Guid.Empty)
        {
          _logger.LogInformation("Invalid invitation ID provided.");
          return null;
        }

        var invitation = await _context.Invitations.FindAsync(id);
        _logger.LogInformation(invitation == null
          ? @$"Invitation with ID {id} not found."
          : @$"Retrieved invitation with ID {id}.");
        return invitation;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, @$"An error occurred while retrieving invitation with ID {id}.");
        return null;
      }

    }

    public async Task<ICollection<Invitation>> GetInvitationsByEventIdAsync(Guid eventId, int page = 1, int size = 5)
    {
      

      try
      {
        if (eventId == Guid.Empty)
        {
          _logger.LogWarning("Invalid event ID provided.");
          return null;
        }
        if (page < 1 || size < 1|| size> 10)
        {
          _logger.LogInformation("Invalid pagination parameters provided.");
          page = 1;
          size = 5;
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
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while retrieving invitations for event with ID {eventId}.");
        return null;
      }


    }
    public async Task UpdateInvitationAsync(Guid invitationId, InvitationStatus status)
    {
      if (invitationId == Guid.Empty)
      {
        _logger.LogInformation("Invalid invitation ID provided.");
        return;
      }
      
      var invitations = await _context.Invitations.FindAsync(invitationId);
      if (invitations != null)
      {
        invitations.InviteState = status;
        _context.Entry(invitations).State = EntityState.Modified;
        _logger.LogInformation($"Updated invitation with ID {invitationId}.");

      }
      else
      {
        _logger.LogError($"An error occurred while updating invitation with ID {invitationId}.");
      }
      await Task.CompletedTask;
    }
  }
}
