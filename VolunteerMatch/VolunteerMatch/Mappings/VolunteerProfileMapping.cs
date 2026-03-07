using AutoMapper;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;

namespace VolunteerMatch.Mappings
{
    public class VolunteerProfileMapping : Profile
    {
        public VolunteerProfileMapping()
        {
            ValueTransformers.Add<string>(s => s == null ? string.Empty : s.Trim());

            CreateMap<VolunteerProfile, GetMyVolunteerProfileDto>()
                .ForMember(
                    dest => dest.Email, 
                    opt => opt.MapFrom(src => src.Volunteer.Email)
                );

            CreateMap<UpdateVolunteerProfileDto, VolunteerProfile>();

            CreateMap<VolunteerProfile, GetVolunteerProfileDto>()
                .ForMember(
                    dest => dest.VolunteerId,
                    opt => opt.MapFrom(src => src.VolunteerId)
                );
        }
    }
}
