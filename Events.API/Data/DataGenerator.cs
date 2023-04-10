namespace Events.API.Data
{
  public class DataGenerator
  {
    public static List<User> GenerateUsers(int count)
    {
      var testUsers = new Faker<User>()
          .RuleFor(u => u.FirstName, f => f.Name.FirstName())
          .RuleFor(u => u.LastName, f => f.Name.LastName())
          .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
          .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
          .Generate(count);
      return testUsers;
    }

    public static List<Event> GenerateEvents(List<User> users)
    {
      var testEvents = new List<Event>();

      foreach (var user in users)
      {
        if (new Random().Next(1, 4) == 1)
        {
          var userEvents = new Faker<Event>()
              .RuleFor(e => e.OwnerId, user.UserId)
              .RuleFor(e => e.Title, f => f.Lorem.Sentence())
              .RuleFor(e => e.Description, f => f.Lorem.Paragraph())
              .RuleFor(e => e.StartDate, f => f.Date.Future(1))
              .RuleFor(e => e.EndDate, (f, e) => f.Date.FutureOffset(1, e.StartDate).DateTime)
              .RuleFor(e=>e.Location,(f,e)=>f.Address.FullAddress())
              .RuleFor(e=>e.EventType,(f,e)=>f.Random.Enum<EventType>())
              .GenerateBetween(1, 2).ToList();

          testEvents.AddRange(userEvents);
        }
      }

      return testEvents;
    }

    public static List<Participant> GenerateParticipants(List<Event> events, List<User> users)
    {
      var testParticipants = new List<Participant>();

      foreach (var eventItem in events)
      {
        var possibleParticipants = users.Where(u => u.UserId != eventItem.OwnerId).ToList();
        var participantsCount = new Random().Next(1, possibleParticipants.Count + 1);

        var selectedParticipants = possibleParticipants.OrderBy(x => x.UserId).Take(participantsCount).ToList();

        foreach (var participant in selectedParticipants)
        {
          if (new Random().Next(1, 4) == 1)
          {
            testParticipants.Add(new Participant(participant.UserId, eventItem.EventId));
          }
        }
      }

      return testParticipants;
    }

    public static List<Invitation> GenerateInvitations(List<Event> events, List<User> users, List<Participant> participants)
    {
      var testInvitations = new List<Invitation>();

      foreach (var eventItem in events)
      {
        var possibleParticipants = users.Where(u => u.UserId != eventItem.OwnerId).ToList();
        var eventParticipants = participants.Where(p => p.EventId == eventItem.EventId).Select(p => p.UserId).ToList();

        // Generate invitations for users who are not participants
        var nonParticipants = possibleParticipants.Where(np => !eventParticipants.Contains(np.UserId)).ToList();
        foreach (var nonParticipant in nonParticipants)
        {
          if (new Random().Next(1, 4) == 1)
          {
            var invitationStatus = InvitationStatus.Pending;
            var inviterUser = users.FirstOrDefault(u => u.UserId == eventItem.OwnerId);
            var invitationMessage = $"{inviterUser.UserName} has invited you to {eventItem.Title}.";
            var inviterUserName = inviterUser.UserName;

            testInvitations.Add(new Invitation(eventItem.EventId, nonParticipant.UserId, invitationStatus, invitationMessage, inviterUserName));
          }
        }
      }

      return testInvitations;
    }
  }
}
