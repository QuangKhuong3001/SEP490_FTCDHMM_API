using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos
{
    public class CreateIngredientCategoryRequest
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên phân loại nguyên liệu không được để trống")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Tên phân loại phải từ 1 đến 150 ký tự")]
        public required string Name { get; set; }
    }
}
