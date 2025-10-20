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
            CreateMap<NotificationUser, NotificationUserDTO>()
                .ForMember(dest => dest.NotificationDTO,
                           opt => opt.MapFrom(src => src.Notification)); 
            CreateMap<CreateNotificationUserDTO, NotificationUser>()
                .ForMember(dest => dest.IsNotificationViewed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsNotificationDeleted, opt => opt.MapFrom(src => false)); ;
            CreateMap<UpdateNotificationUserDTO, NotificationUser>()
                // Ignore null values during mapping to prevent overwriting existing data with null
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}