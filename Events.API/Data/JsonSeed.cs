using System.Runtime.Serialization;
using System.Text.Json.Serialization;

public class JsonSeed
{
    private readonly ILogger<JsonSeed> _logger;
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public JsonSeed(ILogger<JsonSeed> logger,DataContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    public async Task ExportDatabaseToJsonAsync(string fileName)
    {
        _logger.LogInformation(@$"Exporting database to JSON file: {fileName}");
        var users = await _context.Users
            .Include(u => u.OwnedEvents)
            .ThenInclude(e => e.Participants)
            .Include(u => u.OwnedEvents)
            .ThenInclude(e => e.Invites)
            .Include(u => u.RegisteredEvents)
            .Include(u => u.Invites)
            .ToListAsync();

        var userDto = _mapper.Map<IEnumerable<UsersDto>>(users);

        var usersString = JsonSerializer.Serialize(userDto,
            new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve});
        await File.WriteAllTextAsync(fileName, usersString);

        _logger.LogInformation(@$"Database export to JSON file completed: {fileName}");
    }

    public async Task SeedDatabaseFromJsonAsync(DataContext context, string fileName)
    {
        _logger.LogInformation(@$"Seeding database from JSON file: {fileName}");

        var jsonString = await File.ReadAllTextAsync(fileName);

        var users = JsonSerializer.Deserialize<List<User>>(jsonString, new JsonSerializerOptions
        {
            ReferenceHandler =ReferenceHandler.Preserve
        });

        if (users != null)
        {
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        _logger.LogInformation(@$"Database seeding from JSON file completed: {fileName}");
    }
}