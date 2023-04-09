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
            u.FirstName,
            u.LastName,
            u.UserName,
            u.Email,
            OwnedEvents = u.OwnedEvents.Select(e => new
            {
                e.Id,
                e.OwnerId,
                e.Title,
                e.Description,
                e.StartDate,
                e.EndDate
            }),
            ParticipantEvents = u.ParticipantEvents.Select(e => e.Id),
            Invites = u.Invites.Select(i => new
            {
                i.Id,
                i.EventId,
                i.InvitedId,
                i.InviteState
            })
        }).ToList();

        var usersString = JsonSerializer.Serialize(userDtoLite,
            new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve });

        await File.WriteAllTextAsync("./Data/UserDataSeed.json", usersString);

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

        var jsonString = await File.ReadAllTextAsync("./Data/UserDataSeed.json");

        var userDtoLite = JsonSerializer.Deserialize<List<UsersExportDto>>(jsonString, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        });

        // Map the UserSeedDto objects to User objects
        var users = _mapper.Map<IEnumerable<User>>(userDtoLite);

        if (users != null)
        {
            foreach(var user in users)
            {
                user.Id = Guid.NewGuid();

                if(user.OwnedEvents != null)
                {
                    foreach(var ownedEvent in user.OwnedEvents)
                    {
                        ownedEvent.Id = Guid.NewGuid();
                        ownedEvent.OwnerId = user.Id;
                    }
                }

                if(user.RegisteredEvents != null)
                {
                    foreach(var registeredEvent in user.RegisteredEvents)
                    {
                        registeredEvent.UserId = user.Id;
                    }
                }

                if(user.Invites != null)
                {
                    foreach(var invite in user.Invites)
                    {
                        invite.Id = Guid.NewGuid();
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
