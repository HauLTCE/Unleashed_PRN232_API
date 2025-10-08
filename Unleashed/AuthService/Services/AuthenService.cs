

using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;
using AuthService.Repositories.IRepositories;
using AuthService.Services.IServices;
using AuthService.Utilities;
using AuthService.Utilities.IUtilities;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthenService(
            IUserRepository userRepository,
            IUserService userService,
            IJwtTokenGenerator jwtTokenGenerator
            )
        {
            _userRepository = userRepository;
            _userService = userService;
            _jwtTokenGenerator = jwtTokenGenerator;
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

                if (!user.IsUserEnabled.Value || !HashingPassword.VerifyPassword(loginDto.Password, user.UserPassword))
                {
                    return null; // Invalid password or user disabled
                }

                // 3. Credentials are valid, generate a JWT using the service
                var token = _jwtTokenGenerator.GenerateToken(user);

                var response = new LoginUserResponeDTO
                {
                    UserId = user.UserId,
                    Token = token // Use the real token
                };

                return response;
            }
        }
    }

    
