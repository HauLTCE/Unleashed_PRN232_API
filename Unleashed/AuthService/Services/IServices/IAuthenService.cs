using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;
using System;
using System.Threading.Tasks;

namespace AuthService.Services.IServices
{
    /// <summary>
    /// Service responsible for handling user authentication, registration, and account management.
    /// </summary>
    public interface IAuthenService
    {
        /// <summary>
        /// Registers a new user in the system using traditional credentials.
        /// </summary>
        /// <param name="createUserDto">The user registration data.</param>
        /// <returns>A DTO representing the newly created user, or null if registration fails.</returns>
        Task<UserDTO?> RegisterAsync(CreateUserDTO createUserDto);

        /// <summary>
        /// Authenticates a user with email and password, providing a JWT upon success.
        /// </summary>
        /// <param name="loginUserDto">The user's login credentials.</param>
        /// <returns>A DTO containing the JWT and user ID, or null if authentication fails.</returns>
        Task<LoginUserResponeDTO?> LoginAsync(LoginUserDTO loginUserDto);

        /// <summary>
        /// Authenticates a user using a Google ID token. If the user doesn't exist, a new account is created.
        /// </summary>
        /// <param name="googleLoginDto">The DTO containing the Google ID token.</param>
        /// <returns>A DTO containing the JWT and user ID, or null if authentication fails.</returns>
        Task<LoginUserResponeDTO?> LoginWithGoogleAsync(GoogleLoginDTO googleLoginDto);

        /// <summary>
        /// Initiates the password reset process for a user by sending a reset token to their email.
        /// </summary>
        /// <param name="forgotPasswordDto">The DTO containing the user's email.</param>
        /// <returns>A boolean indicating if the request was successfully processed (e.g., email was found and reset link was sent).</returns>
        Task<bool> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto);

        /// <summary>
        /// Resets a user's password using a valid reset token.
        /// </summary>
        /// <param name="resetPasswordDto">The DTO containing the reset token and the new password.</param>
        /// <returns>A boolean indicating if the password was successfully reset.</returns>
        //Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDto);

        /// <summary>
        /// Changes the password for an authenticated user.
        /// </summary>
        /// <param name="userId">The ID of the user changing their password.</param>
        /// <param name="changePasswordDto">The DTO containing the user's old and new passwords.</param>
        /// <returns>A boolean indicating if the password was successfully changed.</returns>
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDTO changePasswordDto);
        Task<bool> ConfirmEmailAsync(Guid user);
    }
}