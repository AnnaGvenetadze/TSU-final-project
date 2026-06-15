using AutoMapper;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Infrastructure.Mappings
{
    public class VolunteerProfileMapper : Profile
    {
        public VolunteerProfileMapper()
        {
            ValueTransformers.Add<string?>(
                s => string.IsNullOrWhiteSpace(s) ? null : s.Trim()
            );

            CreateMap<VolunteerProfile, GetMyVolunteerProfileDto>()
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Volunteer.Email)
                ).ForMember(
                    dest => dest.VolunteerTagIds,
                    opt => opt.MapFrom(
                        src => src.VolunteerTags.Select(vt => vt.TagId)
                        )
                );
            
            CreateMap<UpdateVolunteerProfileDto, VolunteerProfile>()
                .ForMember(
                    destination => destination.Skills,
                    option => option.Ignore())
                .ForMember(
                    destination => destination.Interests,
                    option => option.Ignore())
                .ForMember(
                    destination => destination.VolunteerSkills,
                    option => option.Ignore())
                .ForMember(
                    destination => destination.VolunteerInterests,
                    option => option.Ignore())
                .ForMember(
                    destination => destination.VolunteerTags,
                    option => option.Ignore()
                );

            CreateMap<VolunteerProfile, GetVolunteerProfileDto>()
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Volunteer.Email)
                )
                .ForMember(
                    dest => dest.VolunteerTagIds,
                    opt => opt.MapFrom(
                        src => src.VolunteerTags.Select(vt => vt.TagId)
                    )
                );

            CreateMap<VolunteerProfile, SearchVolunteerItemDto>()
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Volunteer.Email)
                ).ForMember(// თუ src -> dest ობიექტის ფილდის სახელები განსხვავდება იმაპება
                    dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => src.BirthDate)
                );

            CreateMap<Skill, SelectOptionDto>()
                .ForMember(
                    destination => destination.Id,
                    option => option.MapFrom(source => source.SkillId)
                );

            CreateMap<Interest, SelectOptionDto>()
                .ForMember(
                    destination => destination.Id,
                    option => option.MapFrom(source => source.InterestId)
                );
        }
    }
}
