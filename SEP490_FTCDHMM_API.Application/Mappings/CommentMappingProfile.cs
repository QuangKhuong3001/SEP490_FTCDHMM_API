using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Application.Dtos.CommentMentionDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile()
        {
            CreateMap<Comment, CommentResponse>()
                    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                    .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                    .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom<UniversalImageUrlResolver<Comment, CommentResponse>>())
                    .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
                    .ForMember(dest => dest.Mentions, opt => opt.MapFrom(src => src.Mentions));

            CreateMap<CommentMention, MentionedUserResponse>()
                    .ForMember(dest => dest.MentionedUserId, opt => opt.MapFrom(src => src.MentionedUserId))
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.MentionedUser.Email));
        }
    }
}
