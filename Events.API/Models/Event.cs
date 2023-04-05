

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.API.Models
{
  public class Event
  {
    public Event(Guid ownerId, string title, string description, DateTime startDate, DateTime endDate)
    {
      Id = Guid.NewGuid();
      OwnerId = ownerId;
      Title = title;
      Description = description;
      StartDate = startDate;
      EndDate = endDate;
      Participants = new List<User>();
      Invitations = new List<Invitation>();
    }
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
      public void Configure(EntityTypeBuilder<Event> builder)
      {
        builder.Property(e => e.Id)
        .ValueGeneratedOnAdd();

        builder.Property(e => e.OwnerId)
            .IsRequired();

        builder.Property(e => e.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired();

        builder.HasMany(e => e.Participants);

        builder.HasMany(e => e.Invitations);
      }
    }

    private Guid Id { get; set; }
    private Guid OwnerId { get; set; }
    private string Title { get; set; }
    private string Description { get; set; }
    private DateTime StartDate { get; set; }
    private DateTime EndDate { get; set; }

    private ICollection<User>? Participants { get; set; }

    private ICollection<Invitation>? Invitations { get; set; }
    public Guid GetId()
    {
      return Id;
    }

    public Guid GetOwnerId()
    {
      return OwnerId;
    }
    public void changeOwnerId(Guid newOwnerId)
    {
      this.OwnerId = newOwnerId;
    }
    public string GetTitle()
    {
      return Title;
    }

    public void SetTitle(string title)
    {
      this.Title = title;
    }

    public string GetDescription()
    {
      return Description;
    }

    public void SetDescription(string description)
    {
      this.Description = description;
    }

    public DateTime GetStartDate()
    {
      return StartDate;
    }

    public void SetStartDate(DateTime startDate)
    {
      this.StartDate = startDate;
    }

    public DateTime GetEndDate()
    {
      return EndDate;
    }

    public void SetEndDate(DateTime endDate)
    {
      this.EndDate = endDate;
    }

    public ICollection<User> GetParticipants()
    {
      return Participants;
    }

    public void AddParticipant(User participant)
    {
      this.Participants.Add(participant);
    }

    public void AddParticipants(List<User> participants)
    {
      foreach (var participant in participants)
      {
        this.Participants.Add(participant);
      }
    }
    public void RemoveParticipant(User participant)
    {
      this.Participants.Remove(participant);
    }
    public void RemoveParticipants(List<User> participants)
    {
      foreach (var participant in participants)
      {
        this.Participants.Remove(participant);
      }
    }

    public void SendInvite(Invitation invite)
    {
      this.Invitations.Add(invite);
    }

    public void SendInvites(List<Invitation> listInvites)
    {
      foreach (var invites in listInvites)
      {
        this.Invitations.Add(invites);
      }
    }

    public ICollection<Invitation> GetInvites()
    {
      return this.Invitations;
    }
    public Invitation? GetInvites(Guid inviteId)
    {
      return this.Invitations.FirstOrDefault((invite) => invite.GetId() == inviteId);
    }
    public void RemoveInvite(Invitation invite)
    {
      this.Invitations.Remove(invite);
    }
    public void RemoveInvites(List<Invitation> listInvites)
    {
      foreach (var invites in listInvites)
      {
        this.Invitations.Remove(invites);
      }
    }
  }


}