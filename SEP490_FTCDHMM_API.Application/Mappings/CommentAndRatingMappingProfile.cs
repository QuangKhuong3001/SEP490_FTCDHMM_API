using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class CommentAndRatingMappingProfile : Profile
    {
        public CommentAndRatingMappingProfile()
        {

            CreateMap<RatingRequest, Rating>();

            CreateMap<Rating, RatingResponse>()
                .ForMember(dest => dest.UserInteractionResponse,
                    opt => opt.MapFrom(src => src.User));

            CreateMap<CreateCommentRequest, Comment>();

            CreateMap<Comment, CommentResponse>()

                    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                    .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                    .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom<UniversalImageUrlResolver<Comment, CommentResponse>>())
                    .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies));

        }
    }
}
