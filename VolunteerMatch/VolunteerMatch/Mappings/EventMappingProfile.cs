using AutoMapper;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;

namespace VolunteerMatch.Mapping
{
    public class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            CreateMap<CreateEventDetailsDto, Event>();

            CreateMap<UpdateEventDetailsDto, Event>();

            CreateMap<Event, GetEventDetailsDto>()
                .ForMember(dest => dest.OrganizationName,
                    opt => opt.MapFrom(src => src.Organization.OrganizationName));
            //.ForMember(dest => dest.Email,
            //    opt => opt.MapFrom(src => src.Organization.Organization.Email));

            CreateMap<Event, GetEventCardDto>()
                .ForMember(dest => dest.OrganizationName,
                    opt => opt.MapFrom(src => src.Organization.OrganizationName))
                .ForMember(dest => dest.ShortDescription,
                    opt => opt.MapFrom(src => src.Description.Length > 120
                            ? src.Description.Substring(0, 120) + "..."
                            : src.Description));
        }
    }
}