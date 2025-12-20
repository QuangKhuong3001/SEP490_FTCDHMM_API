using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class CommentMentionRepository : EfRepository<CommentMention>, ICommentMentionRepository
    {
        public CommentMentionRepository(AppDbContext context) : base(context) { }
    }
}
