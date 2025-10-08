using AuthService.DTOs.UserDTOs;
using AuthService.Models;
using AuthService.Repositories.IRepositories;
using AuthService.Services.IServices;
using AuthService.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public UserService(IMapper mapper, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<UserDTO?> CreateUser(CreateUserDTO createUserDTO)
        {

            var user = _mapper.Map<User>(createUserDTO);
            user.UserId = Guid.NewGuid();
            user.UserCreatedAt = DateTime.Now;
            user.UserUpdatedAt = DateTime.Now;
            user.UserPassword = HashingPassword.HashPassword(createUserDTO.UserPassword);
            if (await _userRepository.CreateAsync(user))
            {
                return await _userRepository.SaveAsync() ? _mapper.Map<UserDTO>(user) : null;
            }
            return null;
        }

        // --- COMPLETED METHODS START HERE ---

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            var users = _userRepository.All();
            // Map the list of User entities to a list of UserDTOs
            return _mapper.Map<IEnumerable<UserDTO>>(await users.ToListAsync());
        }

        public async Task<UserDTO?> GetById(Guid id)
        {
            var user = await _userRepository.FindAsync(id);
            // Map the found User entity to a UserDTO.
            // If user is null, AutoMapper will correctly return null.
            return _mapper.Map<UserDTO>(user);
        }

        /// <summary>
        /// Updates an existing user.
        /// NOTE: The method signature was changed to include the user's 'id'
        /// to identify which user to update.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="updateUserDTO">The DTO with updated information.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
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
            userToDelete.IsUserEnabled = false;
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
            // Map the found User entity to a UserDTO.
            // If user is null, AutoMapper will correctly return null.
            return _mapper.Map<ImportServiceUserDTO>(user);
        }
    }
}