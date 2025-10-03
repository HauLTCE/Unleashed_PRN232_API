using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;

namespace AuthService.Services.IServices
{
    /// <summary>
    /// Service responsible for handling user authentication and registration.
    /// </summary>
    public interface IAuthenService
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="createUserDto">The user registration data.</param>
        /// <returns>A DTO representing the newly created user, or null if registration fails.</returns>
        Task<UserDTO?> RegisterAsync(CreateUserDTO createUserDto);

        /// <summary>
        /// Authenticates a user and provides a JWT upon success.
        /// </summary>
        /// <param name="loginDto">The user's login credentials.</param>
        /// <returns>A DTO containing the JWT and user id, or null if authentication fails.</returns>
        Task<LoginUserResponeDTO?> LoginAsync(LoginUserDTO loginUserDto);
    }
}