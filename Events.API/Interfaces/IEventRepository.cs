namespace Events.API.Interfaces
{
  public interface IEventRepository
  {
    public Task<ICollection<Event>> GetAllEvents(int page,int size);
    public Task<Event> GetEventById(Guid Id);
    public Task<ICollection<Event>> GetUserEventsAsync(Guid userId);

    public Task CreateEventAsync(Event @event);

    public Task UpdateEventAsync(Event @event);

    public Task DeleteEventAsync(Guid Id);

  }

}