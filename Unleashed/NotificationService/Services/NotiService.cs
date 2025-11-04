using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NotificationService.Clients.IClients;
using NotificationService.DTOs;
using NotificationService.DTOs.External;
using NotificationService.DTOs.NotificationDTOs;
using NotificationService.DTOs.PagedResponse;
using NotificationService.Exceptions;
using NotificationService.Models;
using NotificationService.Repositories.IRepositories;
using NotificationService.Services.IServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class NotiService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationUserRepository _notificationUserRepository;
        private readonly IMapper _mapper;
        private readonly IAuthApiClient _authApiClient;

        private readonly string keyn = "AAHNCO198486";

        public NotiService(
            INotificationRepository notificationRepository,
            INotificationUserRepository notificationUserRepository,
            IMapper mapper,
            IAuthApiClient authApiClient)
        {
            _notificationRepository = notificationRepository;
            _notificationUserRepository = notificationUserRepository;
            _mapper = mapper;
            _authApiClient = authApiClient;
        }

        [Authorize(Roles = "ADMIN, STAFF")]
        public async Task<NotificationDTO?> CreateNotificationForUsers(CreateNotificationForUsersDTO createDto)
        {
            var notification = _mapper.Map<Notification>(createDto);
            if (!await _notificationRepository.CreateAsync(notification))
            {
                return null;
            }
            if (!await _notificationRepository.SaveAsync())
            {
                return null;
            }

            var users = new List<UserDto>();
            foreach (var username in createDto.Usernames.Distinct())
            {
                var user = await _authApiClient.GetUserByUsernameAsync(username);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            if (!users.Any())
            {
                return _mapper.Map<NotificationDTO>(notification);
            }

            foreach (var user in users)
            {
                var notificationUser = new NotificationUser
                {
                    NotificationId = notification.NotificationId,
                    UserId = user.UserId
                };
                await _notificationUserRepository.CreateAsync(notificationUser);
            }

            if (await _notificationUserRepository.SaveAsync())
            {
                return _mapper.Map<NotificationDTO>(notification);
            }

            return null; 
        }

        [Authorize(Roles = "ADMIN, STAFF")]
        public async Task<IEnumerable<NotificationDTO>> GetAllNotifications()
        {
            var notifications = await _notificationRepository.All();
            return _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
        }

        [Authorize]
        public async Task<NotificationDTO?> GetNotificationById(int id)
        {
            var notification = await _notificationRepository.FindAsync(id);
            return _mapper.Map<NotificationDTO>(notification);
        }

        [Authorize(Roles = "ADMIN, STAFF")]
        public async Task<NotificationDTO?> CreateNotification(CreateNotificationDTO createDto)
        {
            var notification = _mapper.Map<Notification>(createDto);

            if (!await _notificationRepository.CreateAsync(notification))
            {
                return null;
            }

            if (await _notificationRepository.SaveAsync())
            {
                return _mapper.Map<NotificationDTO>(notification);
            }

            return null;
        }

        public async Task<NotificationDTO?> CreateNotificationSystem(CreateNotificationDTO createDto, string key)
        {
            if (key == null) throw new NotFoundException("Key not found.");
            if (key != keyn) throw new ForbiddenException("Key not correct.");

            var notification = _mapper.Map<Notification>(createDto);
            var user = await _authApiClient.GetUserByUsernameAsync("System-chan");

            if (user == null) throw new NotFoundException("System credential not found.");

            notification.UserIdSender = user.UserId;

            if (!await _notificationRepository.CreateAsync(notification))
            {
                return null;
            }

            if (await _notificationRepository.SaveAsync())
            {
                return _mapper.Map<NotificationDTO>(notification);
            }

            return null;
        }

        [Authorize(Roles = "ADMIN, STAFF")]
        public async Task<bool> UpdateNotification(int id, UpdateNotificationDTO updateDto)
        {
            var existingNotification = await _notificationRepository.FindAsync(id);
            if (existingNotification == null)
            {
                return false; // Not found
            }

            _mapper.Map(updateDto, existingNotification);
            _notificationRepository.Update(existingNotification);
            return await _notificationRepository.SaveAsync();
        }

        [Authorize(Roles = "ADMIN, STAFF")]
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

        [Authorize(Roles = "ADMIN, STAFF")]
        public async Task<PagedResponse<NotificationDTO>> GetNotificationPagedAsync(int pageNumber, int pageSize, string? searchQuery, bool? isDraft)
        {
            (var notis, var totalRecords) = await _notificationRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                searchQuery,
                isDraft
            );

            var notiDtos = _mapper.Map<List<NotificationDTO>>(notis);

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