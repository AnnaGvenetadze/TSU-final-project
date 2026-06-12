using AutoMapper;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Models;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        ValueTransformers.Add<string?>(
            s => string.IsNullOrWhiteSpace(s) ? null : s.Trim()
        );

        CreateMap<CreateVolunteerDto, VolunteerProfile>()
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

        CreateMap<CreateOrganizationDto, OrganizationProfile>();
    }
}