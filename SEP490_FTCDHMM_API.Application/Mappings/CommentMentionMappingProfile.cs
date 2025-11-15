using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.CommentMentionDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class CommentMentionMappingProfile : Profile
    {
        public CommentMentionMappingProfile()
        {
            CreateMap<CommentMention, MentionedUserResponse>()
                    .ForMember(dest => dest.FirstName,
                        opt => opt.MapFrom(src => src.MentionedUser.FirstName))
                    .ForMember(dest => dest.LastName,
                        opt => opt.MapFrom(src => src.MentionedUser.LastName));
        }
    }
}
