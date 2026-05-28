using AutoMapper;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Infrastructure.Mappings
{
    public class OrganizationProfileMapper : Profile
    {
        public OrganizationProfileMapper()
        {
            ValueTransformers.Add<string?>(
                s => string.IsNullOrWhiteSpace(s) ? null : s.Trim()
            );

            CreateMap<OrganizationProfile, GetMyOrganizationProfileDto>()
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Organization.Email)
                );

            CreateMap<UpdateOrganizationProfileDto, OrganizationProfile>();

            CreateMap<OrganizationProfile, GetOrganizationProfileDto>()
                .ForMember(
                    dest => dest.OrganizationId,
                    opt => opt.MapFrom(src => src.OrganizationId)
                )
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Organization.Email)
                );
        }
    }
}
