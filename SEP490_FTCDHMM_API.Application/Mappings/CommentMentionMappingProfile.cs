using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos.CommentMention;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class CommentMentionMappingProfile : Profile
    {
        public CommentMentionMappingProfile()
        {
            CreateMap<CommentMention, MentionedUserResponse>()
                    .ForMember(dest => dest.UserName,
                        opt => opt.MapFrom(src => src.MentionedUser.UserName));
        }
    }
}
