
namespace Events.API.Profiles;

public class AutoMapperProfiles:Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Event, EventsDto>()
            .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType))
            .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
            .ForMember(dest => dest.Invites, opt => opt.MapFrom(src => src.Invites))
            .ReverseMap();
        CreateMap<User, UsersExportDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.OwnedEvents, opt => opt.MapFrom(src => src.OwnedEvents))
            .ForMember(dest => dest.ParticipantEvents, opt => opt.MapFrom(src => src.RegisteredEvents))
            .ForMember(dest => dest.Invites, opt => opt.MapFrom(src => src.Invites))
            .ReverseMap();
        
     
        CreateMap<Event, RegisterEventDto>()
            .ReverseMap();
        CreateMap<Invitation, InvitationsDto>()
            .ReverseMap();
        CreateMap<Participant, ParticipantsDto>()
            .ReverseMap();
        CreateMap<User, UsersDto>()
            .ReverseMap();
        CreateMap<Invitation, ExportInvitationDto>().ReverseMap();
        CreateMap<Participant, ExportParticipantsDto>().ReverseMap();
        CreateMap<Event, ExportEventDto>().ReverseMap();
        CreateMap<Invitation, CreateInvitationDto>().ReverseMap();
        CreateMap<Participant, RegisterParticipantsDto>().ReverseMap();
        CreateMap<Event, UpdateEventDto>().ReverseMap();

    }
}