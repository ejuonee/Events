using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Interfaces
{
  public interface IInvitationRespository
  {
    Task<ICollection<Invitation>> GetInvitationsByEventIdAsync(Guid eventId);
    Task<Invitation> GetInvitationByIdAsync(Guid id);
    Task CreateInvitationAsync(Invitation invitation);

    Task UpdateInvitationAsync(Guid invitationId, InvitationStatus status);

    Task DeleteInvitationAsync(Guid id);



  }
}