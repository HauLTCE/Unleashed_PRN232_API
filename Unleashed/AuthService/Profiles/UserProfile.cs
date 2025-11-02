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

        CreateMap<User, UserDTO>()
        .ForMember(
        dest => dest.RoleName,
        opt => opt.MapFrom(src => src.Role.RoleName)
        );

        CreateMap<User, ImportServiceUserDTO>();
        // ===================================================================
        //  WRITE MAPPINGS (DTO -> Entity)
        // ===================================================================

        // Mapping for creating a new user from the registration DTO.
        CreateMap<CreateUserDTO, User>();
        CreateMap<CreateExternalUserDTO, User>();
        // Mapping for updating an existing user.
        // This is a crucial configuration for patch/update operations.
        // It tells AutoMapper to only map properties from the source DTO
        // if they are NOT null. This prevents accidentally overwriting
        // existing data in the database with null values.
        CreateMap<UpdateUserDTO, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


        CreateMap<User, UserSummaryDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserUsername, opt => opt.MapFrom(src => src.UserFullname));
    }
}