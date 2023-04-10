using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
  [ApiController]
  [Route("api/events/{eventId}/[controller]")]
  public class InvitationController : ControllerBase
  {
    private readonly ILogger<InvitationController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWork;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;
    public InvitationController(ILogger<InvitationController> logger, IUnitOfWorkRepository unitOfWork, IMemoryCache cache, IMapper mapper)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _cache = cache;
      _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<InvitationsDto>> CreateInvitation(int eventId,
      [FromBody] CreateInvitationDto invitationDto)
    {
      try
      {
        _logger.LogInformation($"Creating a new invitation with {JsonSerializer.Serialize(invitationDto)}");
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
          _logger.LogInformation($"Validation errors occurred while creating the invitation : {errors}");
          _logger.LogInformation(
            $"Invalid data received while creating invitation {JsonSerializer.Serialize(invitationDto)}");
          return BadRequest(new { message = ModelState });
        }

        var invitation = _mapper.Map<Invitation>(invitationDto);
        invitation.EventId = eventId;
        await _unitOfWork.InvitationRespository.CreateInvitationAsync(invitation);
        var result = await _unitOfWork.Complete();
        if (!result)
        {
          _logger.LogError(@$"Unable to create invitation {JsonSerializer.Serialize(invitationDto)} ");
          return BadRequest(new { message = @$"Unable to create invitation {JsonSerializer.Serialize(invitationDto)}
          " });
        }

        var resultInvitationDto = _mapper.Map<InvitationsDto>(invitation);

        _logger.LogInformation($"Created a new invitation for a user with ID {resultInvitationDto.InvitedId} to event {eventId}");

        return CreatedAtAction(nameof(CreateInvitation),
          new { eventId = eventId, userId = resultInvitationDto.InvitedId }, resultInvitationDto);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"Invalid invitation object while creating invitation {JsonSerializer.Serialize(invitationDto)} for event {eventId}");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(ex, $"An error occurred creating invitation {JsonSerializer.Serialize(invitationDto)} for event {eventId}");
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex,
          $"An error occurred while creating invite {JsonSerializer.Serialize(invitationDto)} to event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding participant");
      }
    }

    [HttpDelete("{invitationId}")]
    public async Task<IActionResult> DeleteInvitation(int invitationId, int eventId)
    {
      try
      {
        _logger.LogInformation($"Deleting a  invitation with {invitationId} for event {eventId}");

        var eventExists = await _unitOfWork.EventRepository.GetEventById(eventId);
        if (eventExists == null)
        {
          _logger.LogWarning($"Event with ID {eventId} not found.");
          return NotFound(new { message = $"Event with ID {eventId} not found." });
        }
        await _unitOfWork.InvitationRespository.DeleteInvitationAsync(invitationId);
        var result = await _unitOfWork.Complete();
        if (!result)
        {
          _logger.LogError("Failed to save changes to the database.");
          return BadRequest(new { message = @$"Failed to delete invitation with Id {invitationId}from to the event with id {eventId} from the database" });
        }

        _logger.LogInformation($"Invitation with ID {invitationId} has been successfully removed from event {eventId}.");
        return NoContent();
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"An error occurred while deleting invitation {invitationId} from event {eventId}");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(ex, $"An error occurred while deleting invitation {invitationId} from event {eventId}");
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while deleting invitation {invitationId} from event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing the participant");
      }
    }

    [HttpGet("{invitationId}")]
    public async Task<ActionResult<InvitationsDto>> GetInvitationById(int eventId, int invitationId)
    {
      try
      {
        _logger.LogInformation($"Getting invitation with {invitationId} for event {eventId}");

        var eventExists = await _unitOfWork.EventRepository.GetEventById(eventId);
        if (eventExists == null)
        {
          _logger.LogWarning($"Event with ID {eventId} not found.");
          return NotFound(new { message = $"Event with ID {eventId} not found." });
        }
        var cacheKey = $"invitation_{eventId}_{invitationId}";
        if (!_cache.TryGetValue(cacheKey, out Invitation invitation))
        {
          invitation = await _unitOfWork.InvitationRespository.GetInvitationByInvitationIdandEventIdAsync(invitationId, eventId);
          _cache.Set(cacheKey, invitation, TimeSpan.FromMinutes(1));
        }

        var invitationsDto = _mapper.Map<InvitationsDto>(invitation);
        return Ok(invitationsDto);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting invitation {invitationId} for event {eventId}");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting invitation {invitationId} for event {eventId}");
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting invitation {invitationId} for event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting the participant");
      }
    }

    [HttpPut("{invitationId}")]
    public async Task<IActionResult> UpdateInvitation(UpdateInvitation updateInvitation, int invitationId, int eventId)
    {
      try
      {
        _logger.LogInformation($"Updating invitation with {invitationId} for event {eventId}");

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
          _logger.LogInformation($"Validation errors occurred while updating invitation: {errors}");
          _logger.LogInformation($"Invalid data received while updating invitation {invitationId}");
          return BadRequest(ModelState);
        }

        var invitation = await _unitOfWork.InvitationRespository.GetInvitationByInvitationIdandEventIdAsync(invitationId, eventId);

        if (invitation.InviteState == updateInvitation.InviteState)
        {
          _logger.LogInformation($"No change in invite state for invitation {invitationId}. No update needed.");
          return BadRequest(new { message = $"No change in invite state for invitation {invitationId}. No update needed." });
        }

        invitation.InviteState = updateInvitation.InviteState;
        await _unitOfWork.InvitationRespository.UpdateInvitationAsync(invitation);
        var invitationResult = await _unitOfWork.Complete();

        if (!invitationResult)
        {
          _logger.LogError($"An error occurred while updating invitation {invitationId}");
          return BadRequest(new { message = $"An error occurred while updating invitation {invitationId}" });
        }

        _logger.LogInformation($"Invitation with ID {invitationId} has been successfully updated.");

        if (invitation.InviteState != InvitationStatus.Accepted)
          return NoContent();

        var existingParticipant = await _unitOfWork.ParticipantRepository.GetParticipantByEventAndUserId(eventId, invitation.InvitedId);

        if (existingParticipant != null)
        {
          _logger.LogError($"User with ID {invitation.InvitedId} is already a participant of event {eventId}.");
          return BadRequest($"User with ID {invitation.InvitedId} is already a participant of event {eventId}.");
        }

        var participant = new Participant(invitation.InvitedId, eventId);
        await _unitOfWork.ParticipantRepository.RegisterParticipantAsync(participant);
        var participantResult = await _unitOfWork.Complete();

        if (!participantResult)
        {
          _logger.LogError($"Unable to register participant with ID {participant.ParticipantId} for the event {eventId}.");
          return BadRequest($"Unable to register participant with ID {participant.ParticipantId} for the event {eventId}.");
        }

        _logger.LogInformation($"Participant with ID {participant.ParticipantId} has been successfully registered for the event {eventId}.");
        return NoContent();
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"Invalid invitation ID {invitationId} while updating event {eventId}");
        return BadRequest(new { message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        _logger.LogError(@$"Invitation ID {invitationId} not found for Event: {eventId}");
        return NotFound(new { message = ex.Message + $@"not found for Event: {eventId}" });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while updating invitation {invitationId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the invitation");
      }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvitationsDto>>> GetAllInvitationsByEventId(int eventId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
      try
      {
        _logger.LogInformation($"Getting all invitations for event {eventId}");

        var eventExists = await _unitOfWork.EventRepository.GetEventById(eventId);
        if (eventExists == null)
        {
          _logger.LogWarning($"Event with ID {eventId} not found.");
          return NotFound(new { message = $"Event with ID {eventId} not found." });
        }
        var cacheKey = $"invitations_{eventId}_{page}_Size{pageSize}";
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Invitation> listInvitationsDtos))
        {
          listInvitationsDtos = await _unitOfWork.InvitationRespository.GetInvitationsByEventIdAsync(eventId, page, pageSize);
          _cache.Set(cacheKey, listInvitationsDtos, TimeSpan.FromMinutes(1));
        }

        var invitationsDtos = _mapper.Map<IEnumerable<InvitationsDto>>(listInvitationsDtos);
        return Ok(invitationsDtos);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, $"An error occurred while getting all events");
        return BadRequest(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting all invitations for event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting all invitationg for event {eventId}");
      }
    }


  }
}