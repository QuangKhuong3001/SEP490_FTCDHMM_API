using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class RatingMappingProfile : Profile
    {
        public RatingMappingProfile()
        {
            CreateMap<Rating, RatingResponse>()
                .ForMember(dest => dest.UserInteractionResponse,
                    opt => opt.MapFrom(src => src.User));
        }
    }
}
