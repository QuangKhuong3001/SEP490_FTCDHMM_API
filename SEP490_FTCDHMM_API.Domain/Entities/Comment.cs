namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public Guid? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }

}
