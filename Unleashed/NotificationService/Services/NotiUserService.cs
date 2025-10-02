using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NotificationService.DTOs.NotificationUserDTOs;
using NotificationService.Models;
using NotificationService.Repositories.IRepositories;
using NotificationService.Services.IServices;

namespace NotificationService.Services
{
    public class NotiUserService : INotificationUserService
    {
        private readonly INotificationUserRepository _repository;
        private readonly IMapper _mapper;

        public NotiUserService(INotificationUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationUserDTO>> GetAll()
        {
            var notificationUsers = await _repository.All().ToListAsync();
            return _mapper.Map<IEnumerable<NotificationUserDTO>>(notificationUsers);
        }

        public async Task<NotificationUserDTO?> GetById(int notificationId, Guid userId)
        {
            var notificationUser = await _repository.FindAsync((notificationId, userId));
            return _mapper.Map<NotificationUserDTO>(notificationUser);
        }

        public async Task<NotificationUserDTO?> Create(CreateNotificationUserDTO createDto)
        {
            var notificationUser = _mapper.Map<NotificationUser>(createDto);
            notificationUser.IsNotificationViewed = false;
            notificationUser.IsNotificationDeleted = false;

            await _repository.CreateAsync(notificationUser);

            if (await _repository.SaveAsync())
            {
                return _mapper.Map<NotificationUserDTO>(notificationUser);
            }
            return null;
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
    }
}
