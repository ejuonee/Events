

// namespace Events.API.Repository
// {
//   public class UnitOfWorkRepository : IUnitOfWorkRepository
//   {
//     private readonly DataContext _context;
//     private readonly ILoggerFactory _loggerFactory;
//     private IEventRepository _eventRepository;
//     private IInvitationRespository _invitationRepository;
//     private IParticipantRepository _participantRepository;


//     public UnitOfWorkRepository(DataContext context, ILoggerFactory loggerFactory)
//     {
//       _context = context;
//       _loggerFactory = loggerFactory;

//     }

//     public IEventRepository EventRepository
//     {
//       get { return _eventRepository ??= new EventRepository(_context, _loggerFactory.CreateLogger<EventRepository>()); }
//     }

//     public IInvitationRespository InvitationRespository
//     {
//       get { return _invitationRepository ??= new InvitationRespository(_context, _loggerFactory.CreateLogger<InvitationRespository>()); }
//     }

//     public IParticipantRepository ParticipantRepository
//     {
//       get { return _participantRepository ??= new ParticipantRepository(_context, _loggerFactory.CreateLogger<ParticipantRepository>()); }
//     }

//     public async Task<bool> Complete()
//     {
//       return await _context.SaveChangesAsync() > 0;
//     }
//     public bool HasChanges()
//     {
//       return _context.ChangeTracker.HasChanges();
//     }
//     public void Dispose()
//     {
//       _context.Dispose();
//     }
//   }
// }