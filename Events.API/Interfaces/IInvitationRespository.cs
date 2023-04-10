using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Interfaces
{
  public interface IInvitationRespository
  {
    Task<ICollection<Invitation>> GetInvitationsByEventIdAsync(int eventId, int page, int size);
    Task<Invitation> GetInvitationByInvitationIdandEventIdAsync(int invitationId, int eventId);
    Task CreateInvitationAsync(Invitation invitation);

    Task UpdateInvitationAsync(Invitation invitation);

    Task DeleteInvitationAsync(int id);



  }
}