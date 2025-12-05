using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientDtos
{
    public class CreateIngredientRequest
    {
        [Required(ErrorMessage = "Tên nguyên liệu không được để trống")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Tên nguyên liệu phải từ 1 đến 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phân loại nguyên liệu")]
        public required List<Guid> IngredientCategoryIds { get; set; }

        [Required(ErrorMessage = "Vui lòng tải lên hình ảnh")]
        public required IFormFile Image { get; set; }

        [Required(ErrorMessage = "Vui lòng thêm thành phần dinh dưỡng")]
        public required List<NutrientRequest> Nutrients { get; set; }
    }
}
