using AutoMapper;
using NotificationService.DTOs.NotificationDTOs;
using NotificationService.DTOs.NotificationUserDTOs;
using NotificationService.Models;

namespace NotificationService.Profiles
{
    public class NotificationUserProfile : Profile
    {
        public NotificationUserProfile()
        {
            // Mappings for NotificationUser
            CreateMap<NotificationUser, NotificationUserDTO>();
            CreateMap<CreateNotificationUserDTO, NotificationUser>();
            CreateMap<UpdateNotificationUserDTO, NotificationUser>()
                // Ignore null values during mapping to prevent overwriting existing data with null
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}