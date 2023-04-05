// global using Microsoft.AspNetCore.Mvc;
// global using Microsoft.Extensions.Caching.Memory;

// namespace Events.API.Controllers
// {
//   [ApiController]
//   [Route("api/[controller]")]
//   public class EventController : ControllerBase
//   {
//     private readonly ILogger<EventController> _logger;
//     private readonly IUnitOfWorkRepository _unitOfWork;

//     private readonly IMemoryCache _cache;

//     public EventController(ILogger<EventController> logger, IUnitOfWorkRepository unitOfWork, IMemoryCache cache)
//     {
//       _logger = logger;
//       _unitOfWork = unitOfWork;
//       _cache = cache;
//     }




//     [HttpPost]
//     public async Task<ActionResult<Event>> CreateEvent(Event eventItem)
//     {
//       await _unitOfWork.EventRepository.CreateEventAsync(eventItem);
//       await _unitOfWork.Complete();

//       return CreatedAtAction(nameof(GetEventById), new { id = eventItem.GetId() }, eventItem);
//     }

//     [HttpGet("{id}")]
//     public async Task<ActionResult<Event>> GetEventById(Guid id)
//     {
//       var cacheKey = $"event_{id}";
//       if (!_cache.TryGetValue(cacheKey, out Event eventItem))
//       {
//         eventItem = await _unitOfWork.EventRepository.GetEventById(id);
//         if (eventItem == null)
//         {
//           return NotFound();
//         }
//         _cache.Set(cacheKey, eventItem, TimeSpan.FromMinutes(1));
//       }
//       return Ok(eventItem);
//     }
//     [HttpGet]
//     public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 12)
//     {
//       var cacheKey = $"events_{page}_Size{pageSize}";
//       if (!_cache.TryGetValue(cacheKey, out IEnumerable<Event> events))
//       {
//         events = await _unitOfWork.EventRepository.GetAllEvents();
//         events = events.Skip((page - 1) * pageSize).Take(pageSize).ToList();
//         _cache.Set(cacheKey, events, TimeSpan.FromMinutes(1));
//       }
//       return Ok(events);
//     }
//     [HttpDelete("{id}")]
//     public async Task<IActionResult> DeleteEvent(Guid id)
//     {
//       var eventItem = await _unitOfWork.EventRepository.GetEventById(id);
//       if (eventItem == null)
//       {
//         return BadRequest();
//       }

//       await _unitOfWork.EventRepository.DeleteEventAsync(id);
//       await _unitOfWork.Complete();

//       return NoContent();
//     }

//     [HttpPut("{id}")]
//     public async Task<IActionResult> UpdateEvent(Guid id, Event eventItem)
//     {
//       if (id != eventItem.GetId())
//       {
//         return BadRequest();
//       }

//       await _unitOfWork.EventRepository.UpdateEventAsync(eventItem);
//       await _unitOfWork.Complete();

//       return NoContent();
//     }
//   }
// }