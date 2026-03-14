using AutoMapper;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Infrastructure.Mappings
{
    public class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            ValueTransformers.Add<string>(s => s == null ? string.Empty : s.Trim());

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