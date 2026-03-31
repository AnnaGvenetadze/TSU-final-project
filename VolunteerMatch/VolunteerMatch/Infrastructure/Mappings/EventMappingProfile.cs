using AutoMapper;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Infrastructure.Mappings
{
    public class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            ValueTransformers.Add<string?>(
                s => string.IsNullOrWhiteSpace(s) ? null : s.Trim()
            );

            CreateMap<CreateEventDetailsDto, Event>();
            CreateMap<UpdateEventDetailsDto, Event>();

            CreateMap<Event, GetMyOrgEventDetailsDto>()
                .ForMember(
                    dest => dest.OrganizationName,
                    opt => opt.MapFrom(src => 
                        src.Organization.OrganizationName
                        )
                ).ForMember(
                    dest => dest.EventTagIds,
                    opt => opt.MapFrom(src =>
                        src.EventTags.Select(et => et.TagId)
                        )
                );
            //.ForMember(dest => dest.Email,
            //    opt => opt.MapFrom(src => src.Organization.Organization.Email));

            CreateMap<Event, GetMyOrgEventCardDto>()
                .ForMember(dest => dest.OrganizationName,
                    opt => opt.MapFrom(src => src.Organization.OrganizationName)
                ).ForMember(dest => dest.ShortDescription,
                    opt => opt.MapFrom(src => src.Description.Length > 120
                            ? src.Description.Substring(0, 120) + "..."
                            : src.Description)
                ).ForMember(dest => dest.Theme,
                    opt => opt.MapFrom(src => src.EventTags
                            .OrderBy(et => et.SortOrder)
                            .Select(et => et.Tag.Name)
                            .FirstOrDefault())
                );

            CreateMap<Event, GetEventDetailsDto>()
                .ForMember(dest => dest.OrganizationName,
                    opt => opt.MapFrom(src => src.Organization.OrganizationName)
                ).ForMember(dest => dest.EventTagIds,
                    opt => opt.MapFrom(src => src.EventTags.Select(et => et.TagId))
                );

            CreateMap<Event, GetEventCardDto>()
                .ForMember(dest => dest.OrganizationName,
                    opt => opt.MapFrom(src => src.Organization.OrganizationName)
                ).ForMember(dest => dest.ShortDescription,
                    opt => opt.MapFrom(src => src.Description.Length > 120
                            ? src.Description.Substring(0, 120) + "..."
                            : src.Description)
                ).ForMember(dest => dest.Theme,
                    opt => opt.MapFrom(src => src.EventTags
                        .OrderBy(et => et.SortOrder)
                        .Select(et => et.Tag.Name)
                        .FirstOrDefault())
                );

            CreateMap<Event, GetVolunteerEventCardDto>()
                .IncludeBase<Event, GetEventCardDto>();
        }
    }
}