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

    public async Task<ICollection<Event>> GetAllEvents()
    {
      try
      {
        return await _context.Events.ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in GetAllEvents");
        return null;
      }
    }

    public async Task<Event> GetEventById(Guid Id)
    {
      try
      {
        return await _context.Events.FindAsync(Id);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in GetEventById");
        return null;
      }
    }

    public async Task<ICollection<Event>> GetUserEventsAsync(Guid userId)
    {
      try
      {
        return await _context.Events.Where(e => e.GetOwnerId() == userId).ToListAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in GetUserEventsAsync");
        return null;
      }
    }

    public async Task CreateEventAsync(Event @event)
    {
      try
      {
        await _context.Events.AddAsync(@event);
        _context.Entry(@event).State = EntityState.Added;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in CreateEventAsync");
      }
    }

    public async Task UpdateEventAsync(Event @event)
    {
      try
      {
        _context.Events.Update(@event);
        _context.Entry(@event).State = EntityState.Modified;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in UpdateEventAsync");
      }
    }

    public async Task DeleteEventAsync(Guid Id)
    {
      try
      {
        var @event = await GetEventById(Id);
        if (@event != null)
        {
          _context.Events.Remove(@event);
          _context.Entry(@event).State = EntityState.Deleted;
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in DeleteEventAsync");
      }
    }
  }
}
