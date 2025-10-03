using AutoMapper;
using NotificationService.DTOs.NotificationDTOs;
using NotificationService.Models;

namespace NotificationService.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile() {
            // Maps from Entity to DTO
            CreateMap<Notification, NotificationDTO>();

            // Maps from DTOs to Entity
            CreateMap<CreateNotificationDTO, Notification>();
            CreateMap<UpdateNotificationDTO, Notification>();
        }
    }
}
