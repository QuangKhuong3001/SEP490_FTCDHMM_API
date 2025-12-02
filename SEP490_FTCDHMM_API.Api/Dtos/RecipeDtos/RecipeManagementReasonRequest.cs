using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos
{
    public class RecipeManagementReasonRequest
    {
        [Required(ErrorMessage = "Lí do không được để trống")]
        [MinLength(5, ErrorMessage = "Lí do ít nhất 5 kí tự")]
        [MaxLength(500, ErrorMessage = "Lí do không vượt quá 500 kí tự")]
        public string Reason { get; set; } = string.Empty;
    }
}
