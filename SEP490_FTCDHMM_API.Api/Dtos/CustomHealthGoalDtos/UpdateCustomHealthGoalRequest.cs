using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.NutrientTargetDtos;

namespace SEP490_FTCDHMM_API.Api.Dtos.CustomHealthGoalDtos
{
    public class UpdateCustomHealthGoalRequest
    {
        [Required(ErrorMessage = "Health goal name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "At least one nutrient target is required.")]
        [MinLength(1, ErrorMessage = "Please define at least one nutrient target.")]
        public List<NutrientTargetRequest> Targets { get; set; } = new();
    }
}
