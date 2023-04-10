public class JsonSeed
{
  private readonly ILogger<JsonSeed> _logger;
  private readonly DataContext _context;
  private readonly IMapper _mapper;

  public JsonSeed(ILogger<JsonSeed> logger, DataContext context, IMapper mapper)
  {
    _logger = logger;
    _context = context;
    _mapper = mapper;
  }
  public async Task ExportDatabaseToJsonAsync()
  {
    try
    {
      _logger.LogInformation(@$"Exporting database to JSON file: UserDataSeed.json");

      var users = await _context.Users
          .Include(u => u.OwnedEvents)
              .ThenInclude(e => e.Participants)
          .Include(u => u.OwnedEvents)
              .ThenInclude(e => e.Invites)
          .Include(u => u.RegisteredEvents)
          .Include(u => u.Invites)
          .ToListAsync();

      var userDto = _mapper.Map<IEnumerable<UsersExportDto>>(users);

      var userDtoLite = userDto.Select(u => new
      {
        u.UserId,
        u.FirstName,
        u.LastName,
        u.UserName,
        u.Email,
        OwnedEvents = u.OwnedEvents.Select(e => new
        {
          e.EventId,
          e.OwnerId,
          e.Title,
          e.Description,
          e.StartDate,
          e.EndDate,
          e.Location,
          e.EventType
        }),
        ParticipantEvents = u.ParticipantEvents.Select(e => e.ParticpantId),
        Invites = u.Invites.Select(i => new
        {
          i.InvitationId,
          i.EventId,
          i.InvitedId,
          i.InviteState,
          i.InviterUserName,
          i.InvitationMessage
          
        })
      }).ToList();

      var usersString = JsonSerializer.Serialize(userDtoLite,
          new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve });

      await File.WriteAllTextAsync("UserDataSeed.json", usersString);

      _logger.LogInformation(@$"Database export to JSON file completed: UserDataSeed.json");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while exporting the database to JSON file");
    }
  }

  public async Task SeedDatabaseFromJsonAsync()
  {
    try
    {
      _logger.LogInformation(@$"Seeding database from JSON file: UserDataSeed.json");

      var jsonString = await File.ReadAllTextAsync("UserDataSeed.json");

      var userDtoLite = JsonSerializer.Deserialize<IEnumerable<UsersExportDto>>(jsonString, new JsonSerializerOptions
      {
        ReferenceHandler = ReferenceHandler.Preserve
      });

      // Map the UserSeedDto objects to User objects
      var users = _mapper.Map<IEnumerable<User>>(userDtoLite);

      if (users != null)
      {
        foreach(var user in users)
        {
          if(user.OwnedEvents != null)
          {
            foreach(var ownedEvent in user.OwnedEvents)
            {
              ownedEvent.OwnerId = user.UserId;
            }
          }

          if(user.RegisteredEvents != null)
          {
            foreach(var registeredEvent in user.RegisteredEvents)
            {
              registeredEvent.UserId = user.UserId;
            }
          }

        }

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
      }

      _logger.LogInformation(@$"Database seeding from JSON file completed: UserDataSeed.json");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while seeding the database from JSON file");
    }
  }




}
