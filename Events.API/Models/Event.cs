

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.API.Models
{
  public class Event
  {
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<User>? Participants { get; set; }

    public ICollection<Invitation>? Invitations { get; set; }
  }


}