using AuthService.DTOs.RoleDTOs;

namespace AuthService.Services.IServices
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAll();
        Task<RoleDTO?> GetById(int id);
        Task<RoleDTO?> CreateRole(CreateRoleDTO createRoleDTO);
        Task<bool> UpdateRole(int id, UpdateRoleDTO updateRoleDTO);
        Task<bool> DeleteRole(int id);
    }
}