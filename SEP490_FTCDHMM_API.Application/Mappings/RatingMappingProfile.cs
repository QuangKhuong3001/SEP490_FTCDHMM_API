using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class RatingMappingProfile : Profile
    {
        public RatingMappingProfile()
        {
            CreateMap<Rating, RatingDetailsResponse>()
                .ForMember(dest => dest.IsOwner,
                    opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.UserInteractionResponse,
                    opt => opt.MapFrom(src => src.User));
        }
    }
}
