using AuthService.DTOs.UserDTOs;   
using AuthService.Models; 
using AutoMapper;

namespace AuthService.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Source -> Target

        // ===================================================================
        //  READ MAPPINGS (Entity -> DTO)
        // ===================================================================

        // This maps the main User entity to the UserDto. AutoMapper is smart
        // enough to handle the nested Role and UserRank mappings below.
        CreateMap<User, UserDTO>();

        // ===================================================================
        //  WRITE MAPPINGS (DTO -> Entity)
        // ===================================================================

        // Mapping for creating a new user from the registration DTO.
        CreateMap<CreateUserDTO, User>();

        // Mapping for updating an existing user.
        // This is a crucial configuration for patch/update operations.
        // It tells AutoMapper to only map properties from the source DTO
        // if they are NOT null. This prevents accidentally overwriting
        // existing data in the database with null values.
        CreateMap<UpdateUserDTO, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}