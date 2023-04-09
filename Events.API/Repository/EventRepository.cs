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
    public async Task<ICollection<Event>> GetAllEvents(int page = 1, int size= 5)
    {
      try
      {
        if (page < 1 || size < 1|| size> 10)
        {
          _logger.LogInformation("Invalid pagination parameters provided.");
          page = 1;
          size = 5;
        }
        _logger.LogInformation(@$"Getting all events from the database with pagination (page: {page}, size: {size})");
        var events = await _context.Events
          .OrderByDescending(e => e.StartDate)
          .Include(e=>e.Participants)
          .Include(e=>e.Invites)
          .Skip((page - 1) * size)
          .Take(size)
          .ToListAsync();
        _logger.LogInformation(events.Count == 0
          ? @$"No events found."
          : @$"Successfully retrieved {events.Count} events from the database");
       
        return events;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, @$"Error getting all events from the database with pagination (page: {page}, size: {size})");
        return null;
      }
    }

    public async Task<Event> GetEventById(Guid Id)
    {
      try
      {
        _logger.LogInformation(@$"Getting event with ID {Id} from the database");
        var @event = await _context.Events
          .Include(e => e.Participants)
          .Include(e => e.Invites)
          .FirstOrDefaultAsync(e => e.Id ==Id);


        _logger.LogInformation(@event == null
          ? @$"Event with ID {Id} not found in the database"
          : $"Successfully retrieved event with ID {Id} from the database");
        return @event;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, @$"Error getting event with ID {Id} from the database");
        return null;
      }
    }

    public async Task<ICollection<Event>> GetUserEventsAsync(Guid userId)
    {
      try
      {
        _logger.LogInformation(@$"Getting events for user with ID {userId} from the database");
        var events = await _context.Events
          .Where(e => e.OwnerId == userId)
          .Include(e => e.Participants)
          .Include(e => e.Invites)
          .ToListAsync();
        _logger.LogInformation(@$"Successfully retrieved {events.Count} events for user with ID {userId} from the database");
        return events;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, @$"Error getting events for user with ID {userId} from the database");
        return null;
      }
    }

    public async Task CreateEventAsync(Event @event)
    {
      try
      {
        if (@event == null)
        {
          _logger.LogError("Event cannot be null");
          throw new ArgumentNullException(nameof(@event));
        }

        await _context.Events.AddAsync(@event);
        _logger.LogInformation(@$"Event created with id {@event.Id}");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in CreateEventAsync");
      }

      await Task.CompletedTask;
    }

    public async Task UpdateEventAsync(Event @event)
    {
      try
      {
        if (@event == null)
        {
          throw new ArgumentNullException(nameof(@event), "Event cannot be null");
        }
        _logger.LogInformation(@$"Updating event with ID:{@event.Id}");
        var existingEvent = await _context.Events.FindAsync(@event.Id);

        if (existingEvent == null)
        {
          _logger.LogWarning(@$"Event not found for ID: {@event.Id}");
          throw new ArgumentException(@$"Event not found for ID: {@event.Id}");
        }
        _context.Events.Update(@event);
        _context.Entry(@event).State = EntityState.Modified;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in UpdateEventAsync");
      }

      await Task.CompletedTask;
    }

    public async Task DeleteEventAsync(Guid Id)
    {
      try
      {
        _logger.LogInformation(@$"Deleting event with {Id}" );
        var @event = await _context.Events.FindAsync(Id);
        if (@event == null)
        {
          _logger.LogWarning(@$"Event {Id} not found");
          throw new ArgumentException(@$"Event with ID {Id} not found");
        }
        _context.Events.Remove(@event);
        _logger.LogInformation(@$"Event {Id} deleted");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in DeleteEventAsync");
      }

      await Task.CompletedTask;
    }
  }
}
