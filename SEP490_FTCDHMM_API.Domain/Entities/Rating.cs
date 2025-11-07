using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Rating
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;

        [Range(1, 5, ErrorMessage = "Score must be between 1 and 5.")]
        public int Score { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }

}
