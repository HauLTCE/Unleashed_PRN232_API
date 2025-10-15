using AuthService.DTOs.UserDTOs;

namespace AuthService.Services.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAll();
        Task<UserDTO?> GetById(Guid id);
        Task<ImportServiceUserDTO?> GetByUsernameForImportService(string username);
        Task<UserDTO?> CreateUser(CreateUserDTO createUserDTO);
        Task<UserDTO?> CreateUser(CreateExternalUserDTO createUserDTO);
        Task<bool> UpdateUser(Guid id,UpdateUserDTO updateUserDTO);
        Task<bool> DeleteUser(Guid id);
    }
}
