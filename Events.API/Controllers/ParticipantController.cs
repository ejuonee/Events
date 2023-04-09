namespace Events.API.Controllers
{
  [ApiController]
  [Route("api/events/{eventId}/[controller]")]
  public class ParticipantController : ControllerBase
  {
    private readonly ILogger<ParticipantController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWork;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;

    public ParticipantController(ILogger<ParticipantController> logger, IUnitOfWorkRepository unitOfWork,
      IMemoryCache cache, IMapper mapper)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _cache = cache;
      _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<ParticipantsDto>> AddParticipant(Guid eventId,
      [FromBody] RegisterParticipantsDto participantDto)
    {
      try
      {
        _logger.LogInformation($"Adding a new participant with {JsonSerializer.Serialize(participantDto)}");

        if (!ModelState.IsValid)
        {
          var errors = string.Join(", ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
          _logger.LogInformation($"Validation errors occurred while adding participant: {errors}");
          _logger.LogInformation(
            $"Invalid data received while adding participant {JsonSerializer.Serialize(participantDto)}");
          return BadRequest(ModelState);
        }

        var participant = _mapper.Map<Participant>(participantDto);
        await _unitOfWork.ParticipantRepository.RegisterParticipantAsync (participant,eventId);
        await _unitOfWork.Complete();

        var resultParticipantDto = _mapper.Map<ParticipantsDto>(participant);

        _logger.LogInformation($"Added a new participant with ID {participant.UserId} to event {eventId}");

        return CreatedAtAction(nameof(GetParticipantById),
          new { eventId = eventId, userId = resultParticipantDto.UserId }, resultParticipantDto);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex,
          $"An error occurred while adding participant {JsonSerializer.Serialize(participantDto)} to event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding participant");
      }
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<ParticipantsDto>> GetParticipantById(Guid eventId, Guid userId)
    {
      try
      {
        var cacheKey = $"participant_{eventId}_{userId}";
        if (!_cache.TryGetValue(cacheKey, out Participant participant))
        {
          participant = await _unitOfWork.ParticipantRepository.GetParticipantByIdAsync(eventId, userId);
          if (participant == null)
          {
            return NotFound();
          }

          _cache.Set(cacheKey, participant, TimeSpan.FromMinutes(1));
        }

        var participantDto = _mapper.Map<ParticipantsDto>(participant);
        return Ok(participantDto);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting participant {userId} for event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting the participant");
      }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParticipantsDto>>> GetParticipantsByEventId(Guid eventId, [FromQuery] int page, [FromQuery]int size)
    {
      try
      {
        var cacheKey = $"participants_{eventId}";
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Participant> participants))
        {
          participants = await _unitOfWork.ParticipantRepository.GetParticipantsByEventIdAsync(eventId,page,size);
          _cache.Set(cacheKey, participants, TimeSpan.FromMinutes(1));
        }

        var participantsDto = _mapper.Map<IEnumerable<ParticipantsDto>>(participants);
        return Ok(participantsDto);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting participants for event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError,
          "An error occurred while getting participants for the event");
      }
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> RemoveParticipant(Guid eventId, Guid userId)
    {
      try
      {
        await _unitOfWork.ParticipantRepository.RemoveParticipantAsync(eventId, userId);
        await _unitOfWork.Complete();
        _logger.LogInformation($"Participant with ID {userId} has been successfully removed from event {eventId}.");
        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while removing participant {userId} from event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing the participant");
      }
    }
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<ParticipantsDto>>> RegisterParticipants(Guid eventId, [FromBody] List<RegisterParticipantsDto> participantDtos)
    {
      try
      {
        _logger.LogInformation($"Registering multiple participants for event {eventId}: {JsonSerializer.Serialize(participantDtos)}");

        if (!ModelState.IsValid)
        {
          var errors = string.Join(", ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
          _logger.LogInformation($"Validation errors occurred while registering participants: {errors}");
          _logger.LogInformation($"Invalid data received while registering participants for event {eventId}");
          return BadRequest(ModelState);
        }

        var participants = _mapper.Map<List<Participant>>(participantDtos);
        await _unitOfWork.ParticipantRepository.RegisterParticipantsAsync(participants,eventId);
        await _unitOfWork.Complete();

        var resultParticipantDtos = _mapper.Map<IEnumerable<ParticipantsDto>>(participants);
        _logger.LogInformation($"Registered multiple participants for event {eventId}");

        return CreatedAtAction(nameof(GetParticipantsByEventId), new { eventId = eventId }, resultParticipantDtos);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while registering multiple participants for event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while registering participants");
      }
    }

    
  }
}


