using AutoMapper;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Dtos.Notifications;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Helpers;

namespace VolunteerMatch.Infrastructure.Mappings
{
    public class NotificationMapper : Profile
    {
        public NotificationMapper()
        {
            CreateMap<VolunteerEventMatch, GetMyOrgNotificationDto>()
                .ForMember(
                    dest => dest.VolunteerFullName,
                    opt => opt.MapFrom(src =>
                        src.Volunteer.FirstName + " " + src.Volunteer.LastName))
                .ForMember(
                    dest => dest.Message,
                    opt => opt.MapFrom(src =>
                        NotificationMessageBuilder.CreateOrganizationNotificationMessage(src)))
                .ForMember(
                    dest => dest.Event,
                    opt => opt.MapFrom(src => src.Event));
        }
    }
}
