

var filename = "UserSeedData.json";


var builder = WebApplication.CreateBuilder(args);

//Adding Logging 
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
// getting the connection string from config
var config = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json")
      .Build();

var connectionString = config.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connectionString));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddScoped<IEventRepository, EventRepository>(); 
builder.Services.AddScoped<IInvitationRespository, InvitationRespository>();
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();


var app = builder.Build();

//Seed Data

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<DataContext>();
        var logger = services.GetRequiredService<ILogger<Seed>>();
        var seeder = new Seed(context, logger);
        await seeder.ClearDatabaseAsync();
        //await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<DataContext>();
        var logger = services.GetRequiredService<ILogger<JsonSeed>>();
        var mapper = services.GetRequiredService<IMapper>();
        var jsonSeed = new JsonSeed(logger, context, mapper);
        //await jsonSeed.ExportDatabaseToJsonAsync();
        await jsonSeed.SeedDatabaseFromJsonAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
