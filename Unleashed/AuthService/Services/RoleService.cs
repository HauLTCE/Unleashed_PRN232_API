using AutoMapper;
using AuthService.DTOs.RoleDTOs;
using AuthService.Models;
using AuthService.Repositories.IRepositories;
using AuthService.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDTO>> GetAll()
        {
            var roles = await _roleRepository.All();
            return _mapper.Map<IEnumerable<RoleDTO>>(roles);
        }

        public async Task<RoleDTO?> GetById(int id)
        {
            var role = await _roleRepository.FindAsync(id);
            if (role == null)
            {
                return null;
            }
            return _mapper.Map<RoleDTO>(role);
        }

        public async Task<RoleDTO?> CreateRole(CreateRoleDTO createRoleDTO)
        {
            var role = _mapper.Map<Role>(createRoleDTO);

            await _roleRepository.CreateAsync(role);

            if (await _roleRepository.SaveAsync())
            {
                // Map back to DTO to return the created entity with its new ID
                return _mapper.Map<RoleDTO>(role);
            }

            return null; // Creation failed
        }

        public async Task<bool> UpdateRole(int id, UpdateRoleDTO updateRoleDTO)
        {
            var existingRole = await _roleRepository.FindAsync(id);
            if (existingRole == null)
            {
                return false; // Role not found
            }

            // Map the updated fields from the DTO to the existing entity
            _mapper.Map(updateRoleDTO, existingRole);

            _roleRepository.Update(existingRole);
            return await _roleRepository.SaveAsync();
        }

        public async Task<bool> DeleteRole(int id)
        {
            var roleToDelete = await _roleRepository.FindAsync(id);
            if (roleToDelete == null)
            {
                return false; // Role not found
            }

            _roleRepository.Delete(roleToDelete);
            return await _roleRepository.SaveAsync();
        }
    }
}