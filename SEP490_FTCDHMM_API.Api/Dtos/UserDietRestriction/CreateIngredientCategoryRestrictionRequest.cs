using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserDietRestriction
{
    public class CreateIngredientCategoryRestrictionRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn một nguyên liệu hoặc danh mục nguyên liệu")]
        public Guid IngredientCategoryId { get; set; }

        [Required(ErrorMessage = "Loại hạn chế không được để trống")]
        public string Type { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Ghi chú không được vượt quá 255 ký tự")]
        public string? Notes { get; set; }

        [ModelBinder(BinderType = typeof(SafeNullableDateTimeBinder))]
        public DateTime? ExpiredAtUtc { get; set; }
    }
}
