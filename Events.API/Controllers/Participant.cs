
// namespace Events.API.Controllers
// {
//   [ApiController]
//   [Route("api/events/{eventId}/participants")]
//   public class Participant : ControllerBase
//   {
//     private readonly ILogger<Participant> _logger;
//     private readonly IUnitOfWorkRepository _unitOfWork;

//     private readonly IMemoryCache _cache;
//     public Participant(ILogger<Participant> logger, IUnitOfWorkRepository unitOfWork, IMemoryCache cache)
//     {
//       _logger = logger;
//       _unitOfWork = unitOfWork;
//       _cache = cache;
//     }

//     [HttpGet]
//     public async Task<ActionResult<IEnumerable<Participant>>> GetParticipants(Guid eventId)
//     {
//       var participants = await _unitOfWork.ParticipantRepository.GetParticipantsByEventIdAsync(eventId);
//       //var participantDtos = _mapper.Map<IEnumerable<ParticipationDto>>(participants);

//       return Ok(participants);
//     }


//   }
// }