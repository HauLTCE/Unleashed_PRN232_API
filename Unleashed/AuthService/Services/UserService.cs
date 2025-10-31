using AuthService.DTOs.PagedResponse;
using AuthService.DTOs.UserDTOs;
using AuthService.Models;
using AuthService.Repositories.IRepositories;
using AuthService.Services.IServices;
using AuthService.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository; //khong dung, tai sao them vao
        private readonly IMapper _mapper;

        public UserService(IMapper mapper, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<UserDTO?> CreateUser(CreateUserDTO createUserDTO)
        {
            if(await _userRepository.GetByUsername(createUserDTO.UserUsername) != null || 
                await _userRepository.GetByEmail(createUserDTO.UserEmail) != null) 
                return null;

            var user = _mapper.Map<User>(createUserDTO);
            user.UserId = Guid.NewGuid();
            user.UserCreatedAt = DateTime.Now;
            user.UserUpdatedAt = DateTime.Now;
            user.UserPassword = HashingPassword.HashPassword(createUserDTO.UserPassword);
            user.IsUserEnabled = (user.RoleId.Value == 1 || user.RoleId.Value == 3);
            if (await _userRepository.CreateAsync(user))
            {
                return await _userRepository.SaveAsync() ? _mapper.Map<UserDTO>(user) : null;
            }
            return null;
        }

        public async Task<UserDTO?> CreateUser(CreateExternalUserDTO createUserDTO)
        {

            var user = _mapper.Map<User>(createUserDTO);
            user.UserId = Guid.NewGuid();
            user.UserCreatedAt = DateTime.Now;
            user.UserUpdatedAt = DateTime.Now;

            if (await _userRepository.CreateAsync(user))
            {
                return await _userRepository.SaveAsync() ? _mapper.Map<UserDTO>(user) : null;
            }
            return null;
        }


        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            var users = await _userRepository.All();
            // Map the list of User entities to a list of UserDTOs
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }
        public async Task<PagedResponse<UserDTO>> GetUsersPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchQuery)
        {
            // 1. Call the repository to get the raw data and total count.
            //    Notice the repository method is now specific and accepts the parameters.
            (var users, var totalRecords) = await _userRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                searchQuery
            );

            // 2. Perform business logic (mapping) in the service layer.
            var userDtos = _mapper.Map<List<UserDTO>>(users);
            // Or manually: var userDtos = users.Select(u => new UserDTO { ... }).ToList();

            // 3. Create the final PagedResponse.
            var pagedResponse = new PagedResponse<UserDTO>(
                userDtos,
                totalRecords,
                pageNumber,
                pageSize
            );

            return pagedResponse;
        }

        public async Task<UserDTO?> GetById(Guid id)
        {
            var user = await _userRepository.FindAsync(id);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<bool> UpdateUser(Guid id, UpdateUserDTO updateUserDTO)
        {
            // 1. Find the existing user in the database
            var userFromDb = await _userRepository.FindAsync(id);
            if (userFromDb == null)
            {
                // Can't update a user that doesn't exist
                return false;
            }

            // 2. Map the DTO onto the entity fetched from the DB.
            // Your AutoMapper profile will handle updating only the provided fields.
            _mapper.Map(updateUserDTO, userFromDb);
            userFromDb.UserUpdatedAt = DateTime.Now;
            // 3. Mark the entity as updated and save changes.
            if (_userRepository.Update(userFromDb))
            {
                return await _userRepository.SaveAsync();
            }

            return false;
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            // 1. Find the user to delete
            var userToDelete = await _userRepository.FindAsync(id);
            if (userToDelete == null)
            {
                // User not found, nothing to delete
                return false;
            }
            userToDelete.IsUserEnabled = !userToDelete.IsUserEnabled;
            // 2. Pass the entity to the repository for deletion
            if (_userRepository.Update(userToDelete))
            {
                // 3. Save the changes to the database
                return await _userRepository.SaveAsync();
            }

            return false;
        }

        public async Task<ImportServiceUserDTO?> GetByUsernameForImportService(string username)
        {
            var user = await _userRepository.GetByUsername(username);
            return _mapper.Map<ImportServiceUserDTO>(user);
        }

        public async Task<UserDTO?> GetByEmail(string email)
        {
            var user = await _userRepository.GetByEmail(email);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<UserSummaryDTO>> GetUsersByIdsAsync(IEnumerable<Guid> ids)
        {
            var users = await _userRepository.GetByIdsAsync(ids);
            return _mapper.Map<IEnumerable<UserSummaryDTO>>(users);
        }
    }
}