using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.RecipeIngredient
{
    public class RecipeIngredientRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn nguyên liệu")]
        public Guid IngredientId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng nguyên liệu")]
        [Range(0.1, 10000, ErrorMessage = "Số lượng phải từ 0.1-10000 gram")]
        public decimal QuantityGram { get; set; }
    }
}
