using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.NotificationDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<CreateNotificationRequest, Notification>();

            CreateMap<Notification, NotificationResponse>()
                .ForMember(dest => dest.Senders, opt => opt.Ignore());
        }
    }
}
