namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class CommentMention
    {
        public Guid CommentId { get; set; }
        public Comment Comment { get; set; } = null!;

        public Guid MentionedUserId { get; set; }
        public AppUser MentionedUser { get; set; } = null!;
    }

}
