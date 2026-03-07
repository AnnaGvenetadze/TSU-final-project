using AutoMapper;
using VolunteerMatch.Models;
using VolunteerMatch.Dtos;

namespace VolunteerMatch.Mappings
{   // TODO: ივენთების ფუნქციონალის შექმნის მერე ივენთების ლისტიც უნდა დაბრუნდეს
    public class OrganizationProfileMapping : Profile
    {
        public OrganizationProfileMapping()
        {
            ValueTransformers.Add<string>(s => s == null ? null : s.Trim());

            CreateMap<OrganizationProfile, GetOrganizationProfileDto>()
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Organization.Email)
                );

            CreateMap<UpdateOrganizationProfileDto, OrganizationProfile>();
        }
    }
}
