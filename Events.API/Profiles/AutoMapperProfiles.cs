
namespace Events.API.Profiles;

public class AutoMapperProfiles:Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Event, EventsDto>()
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
        CreateMap<Invitation, ExportInvitationDto>();
        CreateMap<Participant, ExportParticipantsDto>();
        CreateMap<Event, ExportEventDto>();

    }
}