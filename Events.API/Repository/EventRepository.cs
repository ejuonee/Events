namespace Events.API.Repository
{
  public class EventRepository : IEventRepository
  {
    private readonly DataContext _context;
    private readonly ILogger<EventRepository> _logger;


    public EventRepository(DataContext context, ILogger<EventRepository> logger)
    {
      _context = context;
      _logger = logger;
    }
    public async Task<ICollection<Event>> GetAllEvents(int page = 1, int size = 5)
    {

      if (page < 1 || size < 1 || size > 10)
      {
        _logger.LogInformation("Invalid pagination parameters provided.");
        page = 1;
        size = 10;
      }
      _logger.LogInformation(@$"Getting all events from the database with pagination (page: {page}, size: {size})");
      var events = await _context.Events
        .OrderByDescending(e => e.StartDate)
        .Include(e => e.Participants)
        .Include(e => e.Invites)
        .Skip((page - 1) * size)
        .Take(size)
        .ToListAsync();
      _logger.LogInformation(events.Count == 0
        ? @$"No events found."
        : @$"Successfully retrieved {events.Count} events from the database");

      return events;
    }

    public async Task<Event> GetEventById(int Id)
    {

      if (!Int32.IsPositive(Id))
      {
        _logger.LogError("Invalid invitation ID provided.");
        throw new ArgumentException("Invalid event ID provided.");

      }
      _logger.LogInformation(@$"Getting event with ID {Id} from the database");
      var @event = await _context.Events
        .Include(e => e.Participants)
        .Include(e => e.Invites)
        .FirstOrDefaultAsync(e => e.EventId == Id);

      if (@event == null)
      {
        _logger.LogError(@$"Event not found for ID: {Id}");
        throw new KeyNotFoundException(@$"Event not found for ID: {Id}");

      }
      _logger.LogInformation($"Successfully retrieved event with ID {Id} from the database");
      return @event;


    }

    public async Task<ICollection<Event>> GetUserEventsAsync(int userId)
    {

      if (!Int32.IsPositive(userId))
      {
        _logger.LogError("Invalid User ID provided.");
        throw new ArgumentException("Invalid USer ID provided.");
      }
      _logger.LogInformation(@$"checking if the user with ID {userId} exists in the database");
      var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
      if (!userExists)
      {
        _logger.LogError($"User with ID {userId} does not exist.");
        throw new KeyNotFoundException($"User with ID {userId} does not exist.");
      }
      _logger.LogInformation(@$"Getting events for user with ID {userId} from the database");
      var events = await _context.Events
        .Where(e => e.OwnerId == userId)
        .Include(e => e.Participants)
        .Include(e => e.Invites)
        .ToListAsync();
      _logger.LogInformation(@$"Successfully retrieved {events.Count} events for user with ID {userId} from the database");
      return events;

    }

    public async Task CreateEventAsync(Event @event)
    {
      if (@event == null)
      {
        _logger.LogError("Event cannot be null");
        throw new ArgumentNullException(nameof(@event));
      }

      await _context.Events.AddAsync(@event);
      _logger.LogInformation(@$"Event created with id {@event.EventId}");

      await Task.CompletedTask;
    }

    public async Task UpdateEventAsync(Event @event)
    {
      if (@event == null)
      {
        _logger.LogError("Event cannot be null");
        throw new ArgumentNullException(nameof(@event), "Event cannot be null");
      }
      _logger.LogInformation(@$"Updating event with ID:{@event.EventId}");
      var existingEvent = await _context.Events.FindAsync(@event.EventId);

      if (existingEvent == null)
      {
        _logger.LogError(@$"Event not found for ID: {@event.EventId}");
        throw new KeyNotFoundException(@$"Event not found for ID: {@event.EventId}");
      }
      existingEvent.OwnerId = @event.OwnerId;
      existingEvent.Title = @event.Title;
      existingEvent.Description = @event.Description;
      existingEvent.StartDate = @event.StartDate;
      existingEvent.EndDate = @event.EndDate;
      existingEvent.Participants = @event.Participants;
      existingEvent.Invites = @event.Invites;
      existingEvent.Location = @event.Location;
      existingEvent.EventType = @event.EventType;

      _context.Events.Update(existingEvent);
      _context.Entry(existingEvent).State = EntityState.Modified;
      await Task.CompletedTask;
    }

    public async Task DeleteEventAsync(int Id)
    {

      if (!Int32.IsPositive(Id))
      {
        _logger.LogError("Invalid event ID provided.");
        throw new ArgumentException("Invalid event ID provided.");
      }
      _logger.LogInformation(@$"Deleting event with {Id}");
      var @event = await _context.Events.FindAsync(Id);
      if (@event == null)
      {
        _logger.LogError(@$"Event {Id} not found");
        throw new KeyNotFoundException(@$"Event with ID {Id} not found");
      }
      _context.Events.Remove(@event);
      _logger.LogInformation(@$"Event {Id} deleted");


      await Task.CompletedTask;
    }
  }
}
