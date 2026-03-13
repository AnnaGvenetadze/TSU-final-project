using AutoMapper;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;

namespace VolunteerMatch.Infrastructure.Mappings
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
                );// Volunteer ველი არაა GetMyVolunteerProfileDto ამიტომ თუ მას უნდა
                  // მისწვდეს ისეთ ფილდს რომელიც სხვა ფილდიდან გადის (Email)
                  // GetMyVolunteerProfileDto.Email უნდა დაიმაპოს VolunteerProfile.Email-თან
                  // გადასაკონვერტირებელი ობიექტის (src == VolunteerProfile) Volunteer ველის მეშვეობით
            CreateMap<UpdateVolunteerProfileDto, VolunteerProfile>();

            CreateMap<VolunteerProfile, GetVolunteerProfileDto>();

            CreateMap<VolunteerProfile, SearchVolunteerItemDto>()
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Volunteer.Email)
                ).ForMember(// თუ src -> dest ობიექტის ფილდის სახელები განსხვავდება იმაპება
                    dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => src.BirthDate)
                );
        }
    }
}
