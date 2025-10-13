namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class UserFollow
    {
        public Guid FollowerId { get; set; }
        public AppUser Follower { get; set; } = null!;
        public Guid FolloweeId { get; set; }
        public AppUser Followee { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
