using System.Security.Claims;
using AutoMapper;
using OrderFlow.Shared.Models.Identity;
using OrderFlow.Shared.Models.Identity.DTOs;

namespace OrderFlow.Shared.Mappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>();
        CreateMap<Claim, ClaimDto>();
    }
}