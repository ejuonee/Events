

namespace Events.API.Models
{
  public class User
  {

    public User(string userName,string firstName, string lastName, string? email)
    {
      UserName = userName;
      Id = Guid.NewGuid();
      FirstName = firstName;
      LastName = lastName;
      Email = email;
    }

    private Guid Id { get; set; }
    private string FirstName { get; set; }

    private string LastName { get; set; }
    private string UserName { get; set; }
    private string? Email { get; set; }


    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
      public void Configure(EntityTypeBuilder<User> builder)
      {
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(u => u.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.UserName)
             .HasMaxLength(20)
             .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(50);

      }
    }


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
      this.Email = newEmail;
    }
    public void ChangeUserName(string newUserName)
    {
      this.UserName = newUserName;
    }
  }
}