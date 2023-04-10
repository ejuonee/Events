

using Events.API.DTO;

namespace Events.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class EventController : ControllerBase
  {
    private readonly ILogger<EventController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWork;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;
    public EventController(ILogger<EventController> logger, IUnitOfWorkRepository unitOfWork, IMemoryCache cache, IMapper mapper)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _cache = cache;
      _mapper = mapper;
    }
    [HttpPost]
    public async Task<ActionResult<EventsDto>> CreateEvent([FromBody] RegisterEventDto eventItemDto)
    {
      try
      {
        _logger.LogInformation($"Creating a new event with  {JsonSerializer.Serialize(eventItemDto)}");

        if (!ModelState.IsValid)
        {
          var errors = string.Join(", ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
          _logger.LogInformation($"Validation errors occurred while creating event: {errors}");
          _logger.LogInformation($"Invalid data received while creating event {JsonSerializer.Serialize(eventItemDto)}");
          return BadRequest(new { message = ModelState });
        }

        var userExists = await _unitOfWork.IsUserExistsAsync(eventItemDto.OwnerId);
        if (!userExists)
        {
          _logger.LogInformation($"User with ID {eventItemDto.OwnerId} does not exist.");
          return BadRequest(new { message = $"User with ID {eventItemDto.OwnerId} does not exist." });
        }
        var eventItem = _mapper.Map<Event>(eventItemDto);
        await _unitOfWork.EventRepository.CreateEventAsync(eventItem);
        var result = await _unitOfWork.Complete();
        if (!result)
        {
          _logger.LogError("Failed to save changes to the database.");
          return BadRequest(new { message = @$"An error occurred while creating event {JsonSerializer.Serialize(eventItem)}" });
        }
        var resultEventDto = _mapper.Map<EventsDto>(eventItem);

        _logger.LogInformation($"Created a new event with ID {eventItem.EventId}");

        return CreatedAtAction(nameof(GetAllEvents), new { id = resultEventDto.EventId }, resultEventDto);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"Invalid event ID while updating event {JsonSerializer.Serialize(eventItemDto)}");
        return BadRequest(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while creating event {JsonSerializer.Serialize(eventItemDto)}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating event");
      }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventsDto>> GetEventById(int id)
    {
      try
      {
        var cacheKey = $"event_{id}";
        if (!_cache.TryGetValue(cacheKey, out Event eventItem))
        {
          eventItem = await _unitOfWork.EventRepository.GetEventById(id);
          _cache.Set(cacheKey, eventItem, TimeSpan.FromMinutes(1));

        }
        var eventItemDto = _mapper.Map<EventsDto>(eventItem);
        return Ok(eventItemDto);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting event {id}");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting event {id}");
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting event {id}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting the event");
      }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<EventsDto>>> GetUserEvents(int userId)
    {
      try
      {
        var userExists = await _unitOfWork.IsUserExistsAsync(userId);
        if (!userExists)
        {
          _logger.LogInformation($"User with ID {userId} does not exist.");
          return BadRequest(new { message = $"User with ID {userId} does not exist." });
        }
        var cacheKey = $"user_events_{userId}";
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Event> userEvents))
        {
          userEvents = await _unitOfWork.EventRepository.GetUserEventsAsync(userId);
          _cache.Set(cacheKey, userEvents, TimeSpan.FromMinutes(1));
        }

        var userEventsDto = _mapper.Map<IEnumerable<EventsDto>>(userEvents);
        return Ok(userEventsDto);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting events for user  {userId}");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting events for user  {userId}");
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting events for user with ID {userId}");
        return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting events for user with ID {userId}: {ex.Message}");
      }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventsDto>>> GetAllEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
      try
      {
        var cacheKey = $"events_{page}_Size{pageSize}";
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Event> events))
        {
          events = await _unitOfWork.EventRepository.GetAllEvents(page, pageSize);
          _cache.Set(cacheKey, events, TimeSpan.FromMinutes(1));
        }

        var eventsDto = _mapper.Map<IEnumerable<EventsDto>>(events);
        return Ok(eventsDto);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting all events");
        return BadRequest(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An error occurred while getting all events");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting all events");
      }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
      try
      {
        await _unitOfWork.EventRepository.DeleteEventAsync(id);
        var result = await _unitOfWork.Complete();
        if (!result)
        {
          _logger.LogError("Failed to save changes to the database.");
          return BadRequest(new { message = @$"Failed to delete event with Id {id}from to the database" });
        }


        _logger.LogInformation($"Event with ID {id} has been successfully deleted.");

        return NoContent();
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"An error occurred while deleting event {id}");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(ex, $"An error occurred while deleting event {id}");
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while deleting event {id}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the event");
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent([FromBody] UpdateEventDto eventItemDto, int id)
    {

      try
      {
        if (!ModelState.IsValid)
        {
          var errors = string.Join(", ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
          _logger.LogInformation($"Validation errors occurred while updating event: {errors}");
          _logger.LogInformation(
            $"Invalid data received while updating event {JsonSerializer.Serialize(eventItemDto)}");
          return BadRequest(ModelState);
        }

        var eventItem = _mapper.Map<Event>(eventItemDto);
        eventItem.EventId = id;
        await _unitOfWork.EventRepository.UpdateEventAsync(eventItem);
        var result = await _unitOfWork.Complete();
        if (!result)
        {
          _logger.LogError(@$"Unable to update event with Id {eventItem.EventId}");
          return BadRequest(new { message = @$"Unable to update event with Id {eventItem.EventId}" });
        }

        _logger.LogInformation($"Event with ID {eventItem.EventId} has been successfully updated.");

        return NoContent();
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"Invalid event ID while updating event {JsonSerializer.Serialize(eventItemDto)}");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(@$"Event not found for ID: {id}");
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while updating event {JsonSerializer.Serialize(eventItemDto)}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the event");
      }
    }
  }
}