// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace Events.API.Repository
// {
//   public class InvitationRespository : IInvitationRespository
//   {
//     private readonly DataContext _context;
//     private readonly ILogger<InvitationRespository> _logger;


//     public InvitationRespository(DataContext context, ILogger<InvitationRespository> logger)
//     {
//       _context = context;
//       _logger = logger;
//     }

//     public async Task CreateInvitationAsync(Invitation invitation)
//     {
//       if (invitation == null)
//       {
//         _logger.LogWarning("Invitation object is null.");
//         return;
//       }

//       try
//       {
//         // Check if the event and user exist
//         var @event = await _context.Events.FindAsync(invitation.GetEventId());
//         var user = await _context.Users.FindAsync(invitation.GetInviteeId());

//         if (@event == null || user == null)
//         {
//           _logger.LogWarning("Event or user does not exist.");
//           return;
//         }
//         await _context.Invitations.AddAsync(invitation);
//         _context.Entry(invitation).State = EntityState.Added;
//         _logger.LogInformation($"Created new invitation with ID {invitation.GetId()}.");
//       }
//       catch (Exception ex)
//       {
//         _logger.LogError(ex, "An error occurred while creating a new invitation.");
//         return;
//       }
//       await Task.CompletedTask;
//     }

//     public async Task DeleteInvitationAsync(Guid id)
//     {
//       if (id == Guid.Empty)
//       {
//         _logger.LogWarning("Invalid invitation ID provided.");
//         return;
//       }

//       var invitation = await _context.Invitations.FindAsync(id);
//       if (invitation == null)
//       {
//         _logger.LogWarning($"Invitation with ID {id} not found.");
//         return;
//       }

//       try
//       {
//         _context.Invitations.Remove(invitation);
//         _context.Entry(invitation).State = EntityState.Deleted;
//         _logger.LogInformation($"Deleted invitation with ID {id}.");
//       }
//       catch (Exception ex)
//       {
//         _logger.LogError(ex, $"An error occurred while deleting invitation with ID {id}.");
//       }
//       await Task.CompletedTask;
//     }

//     public async Task<Invitation> GetInvitationByIdAsync(Guid id)
//     {

//       try
//       {
//         var invitation = await _context.Invitations.FindAsync(id);
//         if (invitation == null)
//         {
//           _logger.LogWarning($"Invitation with ID {id} not found.");
//         }
//         else
//         {
//           _logger.LogInformation($"Retrieved invitation with ID {id}.");
//         }
//         return invitation;
//       }
//       catch (Exception ex)
//       {
//         _logger.LogError(ex, $"An error occurred while retrieving invitation with ID {id}.");
//         return null;
//       }

//     }

//     public async Task<ICollection<Invitation>> GetInvitationsByEventIdAsync(Guid eventId)
//     {
//       if (eventId == Guid.Empty)
//       {
//         _logger.LogWarning("Invalid event ID provided.");
//         return null;
//       }

//       try
//       {
//         var invitations = await _context.Invitations
//             .Where(i => i.GetEventId() == eventId)
//             .ToListAsync();
//         if (!invitations.Any())
//         {
//           _logger.LogWarning($"No invitations found for event with ID {eventId}.");
//         }
//         else
//         {
//           _logger.LogInformation($"Retrieved {invitations.Count()} invitations for event with ID {eventId}.");
//         }

//         return invitations;
//       }
//       catch (Exception ex)
//       {
//         _logger.LogError(ex, $"An error occurred while retrieving invitations for event with ID {eventId}.");
//         return null;
//       }


//     }

//     public async Task UpdateInvitationAsync(Invitation invitation)
//     {
//       if (invitation == null)
//       {
//         _logger.LogWarning("Invitation object is null.");
//         return;
//       }
//       var invitations = await _context.Invitations.FindAsync(invitation.GetId());
//       if (invitations != null)
//       {
//         _context.Entry(invitation).State = EntityState.Modified;

//         _logger.LogInformation($"Updated invitation with ID {invitation.GetId()}.");

//       }
//       else
//       {
//         _logger.LogError($"An error occurred while updating invitation with ID {invitation.GetId()}.");
//       }
//       await Task.CompletedTask;
//     }
//   }
// }
