using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NotificationService.DTOs.NotificationDTOs;
using NotificationService.DTOs.PagedResponse;
using NotificationService.Models;
using NotificationService.Repositories.IRepositories;
using NotificationService.Services.IServices;

namespace NotificationService.Services
{
    public class NotiService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotiService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationDTO>> GetAllNotifications()
        {
            var notifications = await _notificationRepository.All();
            return _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
        }

        public async Task<NotificationDTO?> GetNotificationById(int id)
        {
            var notification = await _notificationRepository.FindAsync(id);
            return _mapper.Map<NotificationDTO>(notification);
        }

        public async Task<NotificationDTO?> CreateNotification(CreateNotificationDTO createDto)
        {
            var notification = _mapper.Map<Notification>(createDto);

            if (!await _notificationRepository.CreateAsync(notification))
            {
                return null; // Failed to add to context
            }

            if (await _notificationRepository.SaveAsync())
            {
                return _mapper.Map<NotificationDTO>(notification);
            }

            return null; // Failed to save
        }

        public async Task<bool> UpdateNotification(int id, UpdateNotificationDTO updateDto)
        {
            var existingNotification = await _notificationRepository.FindAsync(id);
            if (existingNotification == null)
            {
                return false; // Not found
            }

            // Map DTO values onto the existing entity
            _mapper.Map(updateDto, existingNotification);

            _notificationRepository.Update(existingNotification);
            return await _notificationRepository.SaveAsync();
        }

        public async Task<bool> DeleteNotification(int id)
        {
            var notificationToDelete = await _notificationRepository.FindAsync(id);
            if (notificationToDelete == null)
            {
                return false; // Not found
            }

            _notificationRepository.Delete(notificationToDelete);
            return await _notificationRepository.SaveAsync();
        }

        public async Task<PagedResponse<NotificationDTO>> GetNotificationPagedAsync(int pageNumber, int pageSize, string? searchQuery)
        {
            // 1. Call the repository to get the raw data and total count.
            //    Notice the repository method is now specific and accepts the parameters.
            (var notis, var totalRecords) = await _notificationRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                searchQuery
            );

            // 2. Perform business logic (mapping) in the service layer.
            var notiDtos = _mapper.Map<List<NotificationDTO>>(notis);
            // Or manually: var userDtos = users.Select(u => new UserDTO { ... }).ToList();

            // 3. Create the final PagedResponse.
            var pagedResponse = new PagedResponse<NotificationDTO>(
                notiDtos,
                totalRecords,
                pageNumber,
                pageSize
            );

            return pagedResponse;
        }
    }
}
