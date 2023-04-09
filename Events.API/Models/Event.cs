using System.ComponentModel.DataAnnotations.Schema;

namespace Events.API.Models
{
  public class Event
  {
    public Event( Guid ownerId, string title, string description, DateTime startDate, DateTime endDate,
      User owner, ICollection<Participant> participants, ICollection<Invitation> invites)
    {
      OwnerId = ownerId;
      Title = title ?? throw new ArgumentNullException(nameof(title));
      Description = description;
      StartDate = startDate ;
      EndDate = endDate;
      Participants = participants ?? throw new ArgumentNullException(nameof(participants));
      Invites = invites ?? throw new ArgumentNullException(nameof(invites));
    }

    public Event()
    {
      Participants = new List<Participant>();
      Invites = new List<Invitation>();

    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ICollection<Participant> Participants { get; set; }
    public ICollection<Invitation> Invites { get; set; }
  }


}