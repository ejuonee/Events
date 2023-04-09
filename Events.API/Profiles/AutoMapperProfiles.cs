
namespace Events.API.Profiles;

public class AutoMapperProfiles:Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Event, EventsDto>()
            .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
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

    }
}