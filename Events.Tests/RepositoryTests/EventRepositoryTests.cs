

namespace Events.Tests.RepositoryTests;

public class EventRepositoryTests
{
  private DbContextOptions<DataContext> CreateInMemorySqliteOptions()
  {
    string uniqueConnectionString = $"Filename=:memory:{Guid.NewGuid().ToString()}";
    return new DbContextOptionsBuilder<DataContext>()
        .UseSqlite(uniqueConnectionString)
        .Options;
  }
  private void SeedInMemoryDatabase(DbContextOptions<DataContext> options)
  {
    using var context = new DataContext(options);
    context.Database.OpenConnection();
    context.Database.EnsureCreated();
    context.SaveChanges();
    var users = DataGenerator.GenerateUsers(10);
    context.Users.AddRange(users);
    context.SaveChanges();
    var events = DataGenerator.GenerateEvents(users);
    context.Events.AddRange(events);
    context.SaveChanges();
    var participants = DataGenerator.GenerateParticipants(events, users);
    context.Participants.AddRange(participants);
    context.SaveChanges();
    var invitations = DataGenerator.GenerateInvitations(events, users, participants);
    context.Invitations.AddRange(invitations);
    context.SaveChanges();


  }

  [Fact]
  public async Task GetAllEvents_Tests()
  {
    // Arrange
    var options = CreateInMemorySqliteOptions();
    SeedInMemoryDatabase(options);
    var loggerMock = new Mock<ILogger<EventRepository>>();
    await using var context = new DataContext(options);
    var eventRepository = new EventRepository(context, loggerMock.Object);


    // Act
    var events = await eventRepository.GetAllEvents(page: 1, size: 5);

    // Assert
    Assert.NotNull(events);
    Assert.IsType<List<Event>>(events);
    Assert.True(events.Count <= 5);

  }
  [Fact]
  public async Task GetEventById_Tests()
  {
    // Arrange
    var options = CreateInMemorySqliteOptions();
    SeedInMemoryDatabase(options);
    var loggerMock = new Mock<ILogger<EventRepository>>();
    using var context = new DataContext(options);
    var eventRepository = new EventRepository(context, loggerMock.Object);
    //This assumes an eventId of 1 exists
    int eventId = 1;
    int invalidEventId = -1;
    // This assumes an event of 999 does not exist
    int notIntheDatabase = 999;


    // Act
    var eventResult = await eventRepository.GetEventById(eventId);

    // Assert
    Assert.NotNull(eventResult);
    Assert.Equal(eventId, eventResult.EventId);

    //Act and Assert
    await Assert.ThrowsAsync<ArgumentException>(() => eventRepository.GetEventById(invalidEventId));
    await Assert.ThrowsAsync<KeyNotFoundException>(() => eventRepository.GetEventById(notIntheDatabase));

  }
  [Fact]
  public async Task CreateEventAsync_Tests()
  {
    // Arrange
    var options = CreateInMemorySqliteOptions();
    SeedInMemoryDatabase(options);
    var loggerMock = new Mock<ILogger<EventRepository>>();
    using var context = new DataContext(options);
    var eventRepository = new EventRepository(context, loggerMock.Object);
    var users = DataGenerator.GenerateUsers(1);
    context.Users.AddRange(users);
    await context.SaveChangesAsync();
    var newEvent = new Faker<Event>()
      .RuleFor(e => e.OwnerId, users.First().UserId)
      .RuleFor(e => e.Title, f => f.Lorem.Sentence())
      .RuleFor(e => e.Description, f => f.Lorem.Paragraph())
      .RuleFor(e => e.StartDate, f => f.Date.Future(1))
      .RuleFor(e => e.EndDate, (f, e) => f.Date.FutureOffset(1, e.StartDate).DateTime)
      .RuleFor(e => e.Location, (f, e) => f.Address.FullAddress())
      .RuleFor(e => e.EventType, (f, e) => f.Random.Enum<EventType>())
      .Generate(1).First();
    Event emptyEvent = null;

    // Act
    await eventRepository.CreateEventAsync(newEvent);
    await context.SaveChangesAsync();
    var createdEvent = await context.Events.Where(newEvent => newEvent.OwnerId == users.First().UserId).FirstOrDefaultAsync();

    // Assert
    Assert.NotNull(createdEvent);

    Assert.Equal(newEvent.OwnerId, createdEvent.OwnerId);
    Assert.Equal(newEvent.Title, createdEvent.Title);
    Assert.Equal(newEvent.Description, createdEvent.Description);
    Assert.Equal(newEvent.StartDate, createdEvent.StartDate);
    Assert.Equal(newEvent.EndDate, createdEvent.EndDate);
    Assert.Equal(newEvent.Location, createdEvent.Location);
    Assert.Equal(newEvent.EventType, createdEvent.EventType);


    //Act and Assert
    await Assert.ThrowsAsync<ArgumentNullException>(() => eventRepository.CreateEventAsync(emptyEvent));

  }

}