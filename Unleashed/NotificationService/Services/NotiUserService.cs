using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationService.Clients.IClients;
using NotificationService.DTOs.NotificationDTOs;
using NotificationService.DTOs.NotificationUserDTOs;
using NotificationService.DTOs.PagedResponse;
using NotificationService.Models;
using NotificationService.Repositories;
using NotificationService.Repositories.IRepositories;
using NotificationService.Services.IServices;

namespace NotificationService.Services
{
    public class NotiUserService : INotificationUserService
    {
        private readonly INotificationUserRepository _repository;
        private readonly IAuthApiClient _authApiClient;
        private readonly IMapper _mapper;
        private readonly ILogger<NotiUserService> _logger;

        public NotiUserService(INotificationUserRepository repository, IAuthApiClient authApiClient, IMapper mapper, ILogger<NotiUserService> logger)
        {
            _repository = repository;
            _authApiClient = authApiClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<NotificationUserDTO>> GetAll()
        {
            var notificationUsers = await _repository.All();
            return _mapper.Map<IEnumerable<NotificationUserDTO>>(notificationUsers);
        }

        public async Task<NotificationUserDTO?> GetById(int notificationId, Guid userId)
        {
            var notificationUser = await _repository.FindAsync((notificationId, userId));
            return _mapper.Map<NotificationUserDTO>(notificationUser);
        }

        public async Task<IEnumerable<NotificationUserDTO>?> Create(int notificaitonId)
        {
            var result = new List<NotificationUser>();

            try
            {
                var userIds = await _authApiClient.GetCustomers();

                result = [.. userIds.Select(userId => new NotificationUser
                {
                    UserId = userId,
                    NotificationId = notificaitonId
                })];
                _logger.LogInformation(result.Count.ToString());
                foreach (var item in result)
                {
                    _logger.LogInformation(item.ToString());
                    await _repository.CreateAsync(item);
                }
                var saved = await _repository.SaveAsync();

                return saved
                    ? _mapper.Map<IEnumerable<NotificationUserDTO>>(result)
                    : null;
            }
            catch (Exception ex)
            {
                // TODO: log ex
                return null;
            }
        }


        public async Task<bool> Update(int notificationId, Guid userId, UpdateNotificationUserDTO updateDto)
        {
            var existingRecord = await _repository.FindAsync((notificationId, userId));
            if (existingRecord == null)
            {
                return false; // Not found
            }

            // Map updated fields from DTO to the existing entity
            _mapper.Map(updateDto, existingRecord);
            _repository.Update(existingRecord);

            return await _repository.SaveAsync();
        }

        public async Task<bool> Delete(int notificationId, Guid userId)
        {
            var recordToDelete = await _repository.FindAsync((notificationId, userId));
            if (recordToDelete == null)
            {
                return false; // Not found
            }

            _repository.Delete(recordToDelete);
            return await _repository.SaveAsync();
        }

        public async Task<NotificationUserPagedResponse> GetNotificationUserByUserIdPagedAsync(
     Guid userId,
     int pageNumber,
     int pageSize,
     string? searchQuery)
        {
            // repository should now return (items, totalCount, unviewCount)
            (var notisUser, var totalRecords, var unviewCount) =
                await _repository.GetPagedByUserIdAsync(userId, pageNumber, pageSize, searchQuery);

            var dtoList = _mapper.Map<List<NotificationUserDTO>>(notisUser);

            return new NotificationUserPagedResponse(
                dtoList,
                totalRecords,
                unviewCount,
                pageNumber,
                pageSize
            );
        }

    }
}
