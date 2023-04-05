using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Models
{
  public class Invitation
  {
    private Guid Id { get; set; }
    private Guid EventId { get; set; }
    private Guid InviterId { get; set; }
    private Guid InviteeId { get; set; }
    private InvitationStatus InviteState { get; set; }

    private Event Event { get; set; }

    public Invitation(Guid eventId, Guid inviterId, Guid inviteeId, Event @event)
    {
      Id = Guid.NewGuid();
      EventId = eventId;
      InviterId = inviterId;
      InviteeId = inviteeId;
      InviteState = InvitationStatus.Pending;
      Event = @event;
    }


    public Guid GetId()
    {
      return Id;
    }

    public Guid GetEventId()
    {
      return EventId;
    }

    public Guid GetInviterId()
    {
      return InviterId;
    }
    public Guid GetInviteeId()
    {
      return InviteeId;
    }
    public InvitationStatus GetInviteState()
    {
      return this.InviteState;
    }

    public void ChangeInviteState(InvitationStatus newState)
    {
      this.InviteState = newState;
    }

    public Event GetEvent()
    {
      return Event;
    }
    public enum InvitationStatus
    {
      Pending,
      Accepted,
      Declined
    }
  }


}