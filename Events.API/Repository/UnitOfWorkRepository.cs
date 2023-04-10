

namespace Events.API.Repository
{
  public class UnitOfWorkRepository : IUnitOfWorkRepository
  {
    private readonly DataContext _context;
    private readonly ILoggerFactory _loggerFactory;
    private IEventRepository _eventRepository;
    private IInvitationRespository _invitationRepository;
    private IParticipantRepository _participantRepository;
    private ILogger<UnitOfWorkRepository> _logger;


    public UnitOfWorkRepository(DataContext context, ILoggerFactory loggerFactory, ILogger<UnitOfWorkRepository> logger)
    {
      _context = context;
      _loggerFactory = loggerFactory;
      _logger = logger;

    }

    public IEventRepository EventRepository
    {
      get { return _eventRepository ??= new EventRepository(_context, _loggerFactory.CreateLogger<EventRepository>()); }
    }

    public IInvitationRespository InvitationRespository
    {
      get { return _invitationRepository ??= new InvitationRespository(_context, _loggerFactory.CreateLogger<InvitationRespository>()); }
    }

    public IParticipantRepository ParticipantRepository
    {
      get { return _participantRepository ??= new ParticipantRepository(_context, _loggerFactory.CreateLogger<ParticipantRepository>()); }
    }

    public async Task<bool> Complete()
    {
      for (int i = 0; i < 4; i++)
      {
        try
        {
          var result = await _context.SaveChangesAsync() > 0;
          if (result)
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
          _logger.LogError("Concurrency conflict occurred on attempt {Attempt}. Retrying...", i + 1);

          // Refresh the data in the context
          foreach (var entry in ex.Entries)
          {
            if (entry.Entity != null)
            {
              entry.Reload();
            }
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error saving to the database");
          return false;
        }
      }

      return false;
    }
    public bool HasChanges()
    {
      return _context.ChangeTracker.HasChanges();
    }
    public void Dispose()
    {
      _context.Dispose();
    }
    public async Task<bool> IsUserExistsAsync(int userId)
    {
      try
      {
        _logger.LogInformation($"Checking if user with ID {userId} exists in the database");
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

        _logger.LogInformation(userExists
          ? $"User with ID {userId} exists in the database"
          : $"User with ID {userId} not found in the database");

        return userExists;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error checking if user with ID {userId} exists in the database");
        return false;
      }
    }

  }
}