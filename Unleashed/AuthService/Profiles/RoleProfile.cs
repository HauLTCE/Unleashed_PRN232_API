using AutoMapper;
using AuthService.DTOs.RoleDTOs;
using AuthService.DTOs.UserDTOs;
using AuthService.Models;

namespace AuthService.Profiles;
public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDTO>();

        CreateMap<CreateRoleDTO, Role>();
        CreateMap<UpdateRoleDTO, Role>();
    }
}