

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
          return BadRequest(ModelState);
        }

        var eventItem = _mapper.Map<Event>(eventItemDto);
        await _unitOfWork.EventRepository.CreateEventAsync(eventItem);
        await _unitOfWork.Complete();
        var resultEventDto = _mapper.Map<EventsDto>(eventItem);

        _logger.LogInformation($"Created a new event with ID {eventItem.Id}");

        return CreatedAtAction(nameof(GetEventById), new { id = resultEventDto.Id }, resultEventDto);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while creating event {JsonSerializer.Serialize(eventItemDto)}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating event");
      }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventsDto>> GetEventById(Guid id)
    {
      try
      {
        var cacheKey = $"event_{id}";
        if (!_cache.TryGetValue(cacheKey, out Event eventItem))
        {
          eventItem = await _unitOfWork.EventRepository.GetEventById(id);
          if (eventItem == null)
          {
            return NotFound();
          }
          _cache.Set(cacheKey, eventItem, TimeSpan.FromMinutes(1));
         
        }
        var eventItemDto = _mapper.Map<EventsDto>(eventItem);
        return Ok(eventItemDto);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting event {id}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting the event");
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
          events = await _unitOfWork.EventRepository.GetAllEvents(page,pageSize);
          events = events.Skip((page - 1) * pageSize).Take(pageSize).ToList();
          _cache.Set(cacheKey, events, TimeSpan.FromMinutes(1));
        }

        var eventsDto = _mapper.Map<IEnumerable<EventsDto>>(events);
        return Ok(eventsDto);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An error occurred while getting all events");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting all events");
      }
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
      try
      {
        var eventItem = await _unitOfWork.EventRepository.GetEventById(id);
        if (eventItem == null)
        {
          return BadRequest();
        }

        await _unitOfWork.EventRepository.DeleteEventAsync(id);
        await _unitOfWork.Complete();

        _logger.LogInformation($"Event with ID {id} has been successfully deleted.");

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while deleting event {id}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the event");
      }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEvent( EventsDto eventItemDto)
    {
      
      try
      {
        if (!ModelState.IsValid)
        {
          var errors = string.Join(", ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
          _logger.LogInformation($"Validation errors occurred while updating event: {errors}");
          _logger.LogInformation($"Invalid data received while updating event {eventItemDto.Id}");
          return BadRequest(ModelState);
        }

        var eventItem = _mapper.Map<Event>(eventItemDto);
        await _unitOfWork.EventRepository.UpdateEventAsync(eventItem);
        await _unitOfWork.Complete();

        _logger.LogInformation($"Event with ID {eventItemDto.Id} has been successfully updated.");

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while updating event {eventItemDto.Id}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the event");
      }
    }
  }
}