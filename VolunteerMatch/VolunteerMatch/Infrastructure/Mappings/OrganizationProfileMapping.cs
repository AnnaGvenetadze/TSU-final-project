using AutoMapper;
using VolunteerMatch.Models;
using VolunteerMatch.Dtos;

namespace VolunteerMatch.Infrastructure.Mappings
{
    public class OrganizationProfileMapping : Profile
    {
        public OrganizationProfileMapping()
        {
            ValueTransformers.Add<string>(s => s == null ? string.Empty : s.Trim());

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
