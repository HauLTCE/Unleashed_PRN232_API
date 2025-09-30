

using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;
using AuthService.Repositories.IRepositories;
using AuthService.Services.IServices;
using AuthService.Utilities;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        //private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthenService(
            IUserRepository userRepository,
            IUserService userService
            //IJwtTokenGenerator jwtTokenGenerator
            )
        {
            _userRepository = userRepository;
            _userService = userService;
            //_jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<UserDTO?> RegisterAsync(CreateUserDTO createUserDto)
        {
            // Delegate user creation to the UserService, which already handles
            // password hashing, mapping, and saving.
            return await _userService.CreateUser(createUserDto);
        }

        public async Task<LoginUserResponeDTO?> LoginAsync(LoginUserDTO loginDto)
        {
            // 1. Find the user by their username or email
            // (Assuming your repository has a method like this)
            var user = await _userRepository.GetByUsername(loginDto.Username);
            if (user == null)
            {
                // User not found
                return null;
            }

            // 2. Verify the password
            if (!HashingPassword.VerifyPassword(loginDto.Password, user.UserPassword))
            {
                // Invalid password
                return null;
            }

            // 3. Credentials are valid, generate a JWT
            //var token = _jwtTokenGenerator.GenerateToken(user);
            var token = "No token right now";

            // 4. Create and return the response
            var response = new LoginUserResponeDTO
            {
                UserId = user.UserId,
                Token = token
            };

            return response;
        }
    }
}
    
