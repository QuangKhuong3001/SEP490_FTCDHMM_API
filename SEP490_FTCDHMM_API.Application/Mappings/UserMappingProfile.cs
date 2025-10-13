using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<AppUser, UserResponse>().
                ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.LockoutEnd.HasValue && src.LockoutEnd.Value > DateTime.UtcNow
                    ? UserStatus.Locked : (src.EmailConfirmed ? UserStatus.Verified : UserStatus.Unverified)));

            CreateMap<AppUser, ProfileDto>()
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => src.Gender.Value));

            CreateMap<AppUser, UserFollowResponse>();
        }
    }
}