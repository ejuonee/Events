

namespace Events.API.Models
{
  public class User
  {
    
    public User(string firstName, string lastName, string username, string? email)
    {
      Id = Guid.NewGuid();
      FirstName = firstName;
      LastName = lastName;
      UserName = username;
      Email = email;
    }

    private Guid Id { get; set; }
    private string FirstName { get; set; }

    private string LastName { get; set; }
    private string UserName { get; set; }
    private string? Email { get; set; }
    

    public Guid GetId()
    {
      return this.Id;
    }

    public string GetFullName()
    {
      return $@"{this.FirstName}  {this.LastName}";
    }

    public string? GetEmail()
    {
      return this.Email;
    }
    public string GetUserName()
    {
      return this.UserName;
    }
    public void ChangeFirstName(string newFirstName)
    {
      this.FirstName = newFirstName;
    }

    public void ChangeLastName(string newLastName)
    {
      this.LastName = newLastName;
    }

    public void ChangeEmail(string newEmail)
    {
      this.Email=newEmail;
    }
    public void ChangeUserName(string newUserName)
    {
      this.UserName = newUserName;
    }
  }
}