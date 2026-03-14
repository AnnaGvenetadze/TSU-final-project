using AutoMapper;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Models;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        ValueTransformers.Add<string>(s => s == null ? string.Empty : s.Trim());

        CreateMap<CreateVolunteerDto, VolunteerProfile>();
        CreateMap<CreateOrganizationDto, OrganizationProfile>();
    }
}