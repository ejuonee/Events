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
    public async Task<ActionResult<ParticipantsDto>> AddParticipant(int eventId,
      [FromBody] RegisterParticipantsDto participantDto)
    {
      try
      {
        _logger.LogInformation($"Adding a new participant with {JsonSerializer.Serialize(participantDto)}");
        var eventExists = await _unitOfWork.EventRepository.GetEventById(eventId);
        if (eventExists == null)
        {
          _logger.LogWarning($"Event with ID {eventId} not found.");
          return NotFound(new { message = $"Event with ID {eventId} not found." });
        }
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
        participant.EventId = eventId;
        await _unitOfWork.ParticipantRepository.RegisterParticipantAsync(participant);

        var result = await _unitOfWork.Complete();
        if (!result)
        {
          _logger.LogError("Failed to save changes to the database.");
          return BadRequest(@$"An error occurred while registering participant {JsonSerializer.Serialize(participantDto)} to event {eventId}");
        }

        var resultParticipantDto = _mapper.Map<ParticipantsDto>(participant);

        _logger.LogInformation($"Added a new participant with ID {participant.UserId} to event {eventId}");

        return CreatedAtAction(nameof(GetParticipantById),
          new { eventId = eventId, participantId = resultParticipantDto.ParticipantId }, resultParticipantDto);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex,
          $"An error occurred while adding participant {JsonSerializer.Serialize(participantDto)} to event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding participant");
      }
    }

    [HttpGet("{participantId}")]
    public async Task<ActionResult<ParticipantsDto>> GetParticipantById(int eventId, int participantId)
    {
      try
      {
        _logger.LogInformation($"Getting participant with {participantId}");
        var eventExists = await _unitOfWork.EventRepository.GetEventById(eventId);
        if (eventExists == null)
        {
          _logger.LogWarning($"Event with ID {eventId} not found.");
          return NotFound(new { message = $"Event with ID {eventId} not found." });
        }
        var cacheKey = $"participant_{eventId}_{participantId}";
        if (!_cache.TryGetValue(cacheKey, out Participant participant))
        {
          participant = await _unitOfWork.ParticipantRepository.GetParticipantByIdAsync(participantId, eventId);
          _cache.Set(cacheKey, participant, TimeSpan.FromMinutes(1));
        }

        var participantDto = _mapper.Map<ParticipantsDto>(participant);
        return Ok(participantDto);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting participant {participantId} from event {eventId}");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting participant {participantId} from event {eventId}");
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting participant {participantId} for event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting the participant");
      }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParticipantsDto>>> GetParticipantsByEventId(int eventId, [FromQuery] int page, [FromQuery] int size)
    {
      try
      {
        _logger.LogInformation($"Getting participants for event with Id {eventId}");
        var eventExists = await _unitOfWork.EventRepository.GetEventById(eventId);
        if (eventExists == null)
        {
          _logger.LogWarning($"Event with ID {eventId} not found.");
          return NotFound(new { message = $"Event with ID {eventId} not found." });
        }
        var cacheKey = $"participants_{eventId}";
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Participant> participants))
        {
          participants = await _unitOfWork.ParticipantRepository.GetParticipantsByEventIdAsync(eventId, page, size);
          _cache.Set(cacheKey, participants, TimeSpan.FromMinutes(1));
        }

        var participantsDto = _mapper.Map<IEnumerable<ParticipantsDto>>(participants);
        return Ok(participantsDto);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting all participant for event {eventId}");
        return BadRequest(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting participants for event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError,
          "An error occurred while getting participants for the event");
      }
    }

    [HttpDelete("{participantId}")]
    public async Task<IActionResult> RemoveParticipant(int eventId, int participantId)
    {
      try
      {
        _logger.LogInformation($"Deleting participant with {participantId}");
        var eventExists = await _unitOfWork.EventRepository.GetEventById(eventId);
        if (eventExists == null)
        {
          _logger.LogWarning($"Event with ID {eventId} not found.");
          return NotFound(new { message = $"Event with ID {eventId} not found." });
        }
        await _unitOfWork.ParticipantRepository.RemoveParticipantAsync(participantId, eventId);
        var result = await _unitOfWork.Complete();
        if (!result)
        {
          _logger.LogError("Failed to save changes to the database.");
          return BadRequest(@$"Failed to delete participant with Id {participantId}from to the database");
        }

        _logger.LogInformation($"Participant with ID {participantId} has been successfully removed from event {eventId}.");
        return NoContent();
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"Invalid Particpant ID {participantId} or  event ID {eventId} while deleting participant");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(@$"Participant not found for ID: {participantId}");
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while removing participant {participantId} from event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing the participant");
      }
    }
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<ParticipantsDto>>> RegisterParticipants(int eventId, [FromBody] List<RegisterParticipantsDto> participantDtos)
    {
      try
      {
        _logger.LogInformation($"Registering multiple participants for event {eventId}: {JsonSerializer.Serialize(participantDtos)}");
        // Check if the event exists
        var eventExists = await _unitOfWork.EventRepository.GetEventById(eventId);
        if (eventExists == null)
        {
          _logger.LogWarning($"Event with ID {eventId} not found.");
          return NotFound(new { message = $"Event with ID {eventId} not found." });
        }

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
        participants.ForEach(p => p.EventId = eventId);
        var failedParticipants = await _unitOfWork.ParticipantRepository.RegisterParticipantsAsync(participants);
        if (participants.Count != failedParticipants.Count)
        {
          var result = await _unitOfWork.Complete();

          if (!result)
          {
            _logger.LogError("Failed to save changes to the database.");
            return BadRequest(@$"An error occurred while registering participants {JsonSerializer.Serialize(participants)} to event {eventId}");
          }
        }
        if (failedParticipants.Count > 0)
        {
          _logger.LogError($"Failed to register these participants {JsonSerializer.Serialize(failedParticipants)} for event {eventId}");
          return StatusCode(StatusCodes.Status206PartialContent, new
          {
            Message = "Some Participants were not registered either because they have been registered or they are not users in the database",
            FailedParticipants = failedParticipants
          });
        }
        var resultParticipantDtos = _mapper.Map<IEnumerable<ParticipantsDto>>(participants);
        _logger.LogInformation($"Registered multiple participants for event {eventId}");

        return CreatedAtAction(nameof(GetParticipantsByEventId), new { eventId = eventId }, resultParticipantDtos);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"Invalid event ID while updating event {eventId}");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(@$"Event not found for ID: {eventId}");
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while registering multiple participants for event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while registering participants");
      }
    }


  }
}


