using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.DTO
{
  public class DTO
  {

  }
  public class EventDTO
  {

    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ICollection<User>? Participants { get; set; }
    public ICollection<Invitation>? Invitations { get; set; }
  }

  public class InvitationDTO
  {
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public User EventOwner { get; set; }
    public User InvitedGuest { get; set; }
    public InvitationStatus InviteState { get; set; }
    public Event Event { get; set; }
  }

  public class ParticipantsDTO
  {
    public Guid EventID { get; set; }
    public Event Event { get; set; }


  }


}