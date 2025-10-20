using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;

        public NotiUserService(INotificationUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

        public async Task<IEnumerable<NotificationUserDTO>?> Create(CreateNotificationUserDTO createDto)
        {
            try
            {
                List <NotificationUser> notificationUsers = [];
                foreach (var createNoti in createDto.UserIds)
                {
                    var notificationUser = _mapper.Map<NotificationUser>(createDto);
                    await _repository.CreateAsync(notificationUser);
                    notificationUsers.Add(notificationUser);
                }

                if (await _repository.SaveAsync())
                {
                    return _mapper.Map<IEnumerable<NotificationUserDTO>>(notificationUsers);
                }
                return null;
            }
            catch (Exception ex)
            {
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

        public async Task<PagedResponse<NotificationUserDTO>> GetNotificationUserByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize, string? searchQuery)
        {
            // 1. Call the repository to get the raw data and total count.
            //    Notice the repository method is now specific and accepts the parameters.
            (var notisUser, var totalRecords) = await _repository.GetPagedByUserIdAsync(
                userId,
                pageNumber,
                pageSize,
                searchQuery
            );

            // 2. Perform business logic (mapping) in the service layer.
            var notiUserDtos = _mapper.Map<List<NotificationUserDTO>>(notisUser);
            // Or manually: var userDtos = users.Select(u => new UserDTO { ... }).ToList();

            // 3. Create the final PagedResponse.
            var pagedResponse = new PagedResponse<NotificationUserDTO>(
                notiUserDtos,
                totalRecords,
                pageNumber,
                pageSize
            );

            return pagedResponse;
        }
    }
}
