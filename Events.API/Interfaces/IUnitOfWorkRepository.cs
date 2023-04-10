using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Interfaces
{
  public interface IUnitOfWorkRepository : IDisposable
  {
    IEventRepository EventRepository { get; }
    IInvitationRespository InvitationRespository { get; }
    IParticipantRepository ParticipantRepository { get; }
    Task<bool> Complete();

    Task<bool> IsUserExistsAsync(int userId);
    bool HasChanges();
  }
}