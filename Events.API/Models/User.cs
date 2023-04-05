

namespace Events.API.Models
{
  public class User
  {

    public User(string userName, string firstName, string lastName, string? email)
    {
      UserName = userName;
      Id = Guid.NewGuid();
      FirstName = firstName;
      LastName = lastName;
      Email = email;
    }

    public Guid Id { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public string UserName { get; set; }
    public string? Email { get; set; }

    public ICollection<Event>? Events { get; set; }

    public ICollection<Invitation>? Invites { get; set; }


  }
}