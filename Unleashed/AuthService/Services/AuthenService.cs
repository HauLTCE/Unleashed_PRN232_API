using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;
using AuthService.Repositories.IRepositories;
using AuthService.Services.IServices;
using AuthService.Utilities;
using AuthService.Utilities.IUtilities;
using AutoMapper;
using Google.Apis.Auth; 
using Microsoft.Extensions.Configuration; // Required for Google Client ID
using System;
using System.Security.Cryptography;


namespace AuthService.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        //private readonly IEmailService _emailService; 

        public AuthenService(
            IUserRepository userRepository,
            IUserService userService,
            IJwtTokenGenerator jwtTokenGenerator,
            IConfiguration configuration,
            IMapper mapper
            //IEmailService emailService
            ) 
        {
            _userRepository = userRepository;
            _userService = userService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _configuration = configuration;
            _mapper = mapper;
            //_emailService = emailService;
        }

        public async Task<UserDTO?> RegisterAsync(CreateUserDTO createUserDto)
        {
            // Delegate user creation to the UserService, which already handles
            // password hashing, mapping, and saving.
            return await _userService.CreateUser(createUserDto); 
        }

        public async Task<LoginUserResponeDTO?> LoginAsync(LoginUserDTO loginDto)
        {
            var user = await _userRepository.GetByUsername(loginDto.Username);
            if (user == null)
            {
                return null; // User not found
            }

            // Verify password and user status
            if (!user.IsUserEnabled.Value || !HashingPassword.VerifyPassword(loginDto.Password, user.UserPassword))
            {
                return null; // Invalid credentials or user disabled
            }

            // Generate JWT
            var token = _jwtTokenGenerator.GenerateToken(user);

            return new LoginUserResponeDTO
            {
                UserId = user.UserId,
                Token = token
            };
        }


        public async Task<LoginUserResponeDTO?> LoginWithGoogleAsync(GoogleLoginDTO googleLoginDto)
        {
            try
            {
                var googleClientId = _configuration["GoogleAuth:ClientId"];
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDto.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { googleClientId } });

                var user = await _userRepository.GetByEmail(payload.Email);

                // If user doesn't exist, create a new one
                if (user == null)
                {
                    await _userService.CreateUser(new CreateExternalUserDTO
                    {
                        UserEmail = payload.Email,
                        UserFullname = payload.Name,
                        UserUsername = payload.Email

                    });

                    if (user == null) return null; // Failed to create user

                    user = await _userRepository.GetByEmail(payload.Email);
                }

                // User exists or was just created, generate a token
                var token = _jwtTokenGenerator.GenerateToken(user);

                return new LoginUserResponeDTO
                {
                    UserId = user.UserId,
                    Token = token
                };
            }
            catch (InvalidJwtException)
            {
                // Token is invalid
                return null;
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto)
        {
            var user = await _userRepository.GetByEmail(forgotPasswordDto.Email);
            if (user == null)
            {
                return true;
            }

            // Generate a secure token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            // Update user record with token and expiry (e.g., expires in 15 minutes)
            // Assumes User entity has PasswordResetToken and ResetTokenExpires properties
            //user.PasswordResetToken = token;
            //user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(15);
            //await _userRepository.UpdateUserAsync(user);

            // Send email
            //await _emailService.SendPasswordResetEmailAsync(user.UserEmail, token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDto)
        {
            // Find user by the non-expired token
            var user = await _userRepository.FindAsync(resetPasswordDto.UserId);

            if (user == null)
            {
                return false; 
            }

            // Hash the new password and update the user
            user.UserPassword = HashingPassword.HashPassword(resetPasswordDto.Password);
            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDTO changePasswordDto)
        {
            var user = await _userRepository.FindAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            // Verify the current password
            if (!HashingPassword.VerifyPassword(changePasswordDto.OldPassword, user.UserPassword))
            {
                return false; // Old password does not match
            }

            // Hash and set the new password
            user.UserPassword = HashingPassword.HashPassword(changePasswordDto.NewPassword);
            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }

        public async Task<bool> ConfirmEmailAsync(Guid userID)
        {
           var user = await _userRepository.FindAsync(userID);
            if (user == null) return false;
                user.IsUserEnabled = true;
                _userRepository.Update(user);
                await _userRepository.SaveAsync();
                return true;
        }
    }
}