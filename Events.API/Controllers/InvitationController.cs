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
    public async Task<ActionResult<InvitationsDto>> CreateInvitation(Guid eventId,
      [FromBody] CreateInvitationDto invitationDto)
    {
      try
      {
        _logger.LogInformation($"Creating a new invitation with {JsonSerializer.Serialize(invitationDto)}");

        if (!ModelState.IsValid)
        {
          var errors = string.Join(", ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
          _logger.LogInformation($"Validation errors occurred while creating the invitation : {errors}");
          _logger.LogInformation(
            $"Invalid data received while creating invitation {JsonSerializer.Serialize(invitationDto)}");
          return BadRequest(ModelState);
        }

        var invitation = _mapper.Map<Invitation>(invitationDto);
        await _unitOfWork.InvitationRespository.CreateInvitationAsync (invitation);
        await _unitOfWork.Complete();

        var resultInvitationDto = _mapper.Map<InvitationsDto>(invitation);

        _logger.LogInformation($"Created a new invitation for a user with ID {resultInvitationDto.InvitedId} to event {eventId}");

        return CreatedAtAction(nameof(CreateInvitation),
          new { eventId = eventId, userId = resultInvitationDto.InvitedId }, resultInvitationDto);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex,
          $"An error occurred while creating invite {JsonSerializer.Serialize(invitationDto)} to event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding participant");
      }
    }
    
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteInvitation( Guid invitationId,Guid eventId)
    {
      try
      {
        await _unitOfWork.InvitationRespository.DeleteInvitationAsync(invitationId);
        await _unitOfWork.Complete();
        _logger.LogInformation($"Invitation with ID {invitationId} has been successfully removed from event {eventId}.");
        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while deleting invitation {invitationId} from event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing the participant");
      }
    }
    
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<InvitationsDto>> GetInvitationById(Guid eventId, Guid invitationId)
    {
      try
      {
        var cacheKey = $"invitation_{eventId}_{invitationId}";
        if (!_cache.TryGetValue(cacheKey, out Invitation invitation))
        {
          invitation = await _unitOfWork.InvitationRespository.GetInvitationByIdAsync(invitationId);
          if (invitation == null)
          {
            return NotFound();
          }

          _cache.Set(cacheKey, invitation, TimeSpan.FromMinutes(1));
        }

        var invitationsDto = _mapper.Map<InvitationsDto>(invitation);
        return Ok(invitationsDto);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting invitation {invitationId} for event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting the participant");
      }
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateInvitationt( InvitationsDto invitationsDto, Guid eventId)
    {
      
      try
      {
        if (!ModelState.IsValid)
        {
          var errors = string.Join(", ", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
          _logger.LogInformation($"Validation errors occurred while updating invitation: {errors}");
          _logger.LogInformation($"Invalid data received while updating invitation {invitationsDto.Id}");
          return BadRequest(ModelState);
        }

        var invitations = _mapper.Map<Invitation>(invitationsDto);
        await _unitOfWork.InvitationRespository.UpdateInvitationAsync(invitations.Id, invitations.InviteState);
        await _unitOfWork.Complete();
        _logger.LogInformation($"Invitation with ID {invitations.Id} has been successfully updated.");

        var particpant = new Participant(invitations.InvitedId,eventId);
        await _unitOfWork.ParticipantRepository.RegisterParticipantAsync(particpant, eventId);
        await _unitOfWork.Complete();
        _logger.LogInformation($"Particpant with ID {particpant.Id} has been successfully registered for the Event {eventId}.");

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while updating invitation {invitationsDto.Id}");
        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the invitation");
      }
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvitationsDto>>> GetAllInvitationsByEventId(Guid eventId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
      try
      {
        var cacheKey = $"invitations_{eventId}_{page}_Size{pageSize}";
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Invitation> listInvitationsDtos))
        {
          listInvitationsDtos = await _unitOfWork.InvitationRespository.GetInvitationsByEventIdAsync(eventId,page,pageSize);
          _cache.Set(cacheKey, listInvitationsDtos, TimeSpan.FromMinutes(1));
        }

        var invitationsDtos = _mapper.Map<IEnumerable<InvitationsDto>>(listInvitationsDtos);
        return Ok(invitationsDtos);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while getting all invitations for event {eventId}");
        return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting all invitationg for event {eventId}");
      }
    }


  }
}