using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientDtos
{
    public class UpdateIngredientRequest
    {
        [Required(ErrorMessage = "Cần xác định thời gian cuối cùng chỉnh sửa mục tiêu.")]
        public DateTime? LastUpdatedUtc { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        public IFormFile? Image { get; set; }

        [Required(ErrorMessage = "Vui lòng thêm thành phần dinh dưỡng")]
        public List<NutrientRequest>? Nutrients { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phân loại nguyên liệu")]
        public List<Guid> IngredientCategoryIds { get; set; } = new();
    }
}
