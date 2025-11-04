using AuthService.DTOs.PagedResponse;
using AuthService.DTOs.UserDTOs;

namespace AuthService.Services.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAll();
        Task<PagedResponse<UserDTO>> GetUsersPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchQuery);
        Task<UserDTO?> GetById(Guid id);
        Task<UserDTO?> GetByEmail(string email);
        Task<ImportServiceUserDTO?> GetByUsernameForImportService(string username);
        Task<UserDTO?> CreateUser(CreateUserDTO createUserDTO);
        Task<UserDTO?> CreateUser(CreateExternalUserDTO createUserDTO);
        Task<bool> UpdateUser(Guid id,UpdateUserDTO updateUserDTO);
        Task<bool> DeleteUser(Guid id);
        Task<IEnumerable<UserSummaryDTO>> GetUsersByIdsAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<Guid>> GetCustomerIds();

        Task<IEnumerable<UserReviewDTO>> GetUserReviewInfosAsync(IEnumerable<Guid> userIds);
    }
}
