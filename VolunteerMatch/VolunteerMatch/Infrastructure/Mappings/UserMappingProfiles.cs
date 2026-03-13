using AutoMapper;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        ValueTransformers.Add<string>(s => s == null ? string.Empty : s.Trim());

        CreateMap<CreateVolunteerDto, VolunteerProfile>();
        CreateMap<CreateOrganizationDto, OrganizationProfile>();
    }
}