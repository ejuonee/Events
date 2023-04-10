

using System.Linq.Expressions;

namespace Events.API.Models
{
  public class User
  {
    public User()
    {
      OwnedEvents = new List<Event>();
      Invites = new List<Invitation>();
      RegisteredEvents = new List<Participant>();
    }


    public User(string firstName, string lastName, string userName, string? email, ICollection<Event>? ownedEvents,
      ICollection<Participant> registeredEvents, ICollection<Invitation>? invites)
    {
      FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
      LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
      UserName = userName ?? throw new ArgumentNullException(nameof(userName));
      Email = email;
      OwnedEvents = ownedEvents ?? throw new ArgumentNullException(nameof(ownedEvents)); ;
      RegisteredEvents = registeredEvents ?? throw new ArgumentNullException(nameof(registeredEvents));
      Invites = invites ?? throw new ArgumentNullException(nameof(invites)); ;
    }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string? Email { get; set; }
    public ICollection<Event>? OwnedEvents { get; set; }
    [JsonPropertyName("ParticipantEvents")]
    public ICollection<Participant> RegisteredEvents { get; set; }

    public ICollection<Invitation> Invites { get; set; }


  }
}