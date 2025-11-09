using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDietRestriction
{
    public class CreateIngredientRestrictionRequest
    {
        [Required]
        public Guid IngredientId { get; set; }

        [Required]
        public required string Type { get; set; }

        [MaxLength(255)]
        public string? Notes { get; set; }

        public DateTime? ExpiredAtUtc { get; set; }
    }
}
