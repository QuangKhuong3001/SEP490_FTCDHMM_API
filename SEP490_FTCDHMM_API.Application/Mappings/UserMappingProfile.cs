using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<AppUser, UserDto>().
            ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                src.LockoutEnd.HasValue && src.LockoutEnd.Value > DateTime.UtcNow
                ? UserStatus.Locked
                : (src.EmailConfirmed ? UserStatus.Verified : UserStatus.Unverified)));
    }
}
