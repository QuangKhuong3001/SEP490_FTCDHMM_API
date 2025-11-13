using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction
{
    public class CreateIngredientRestrictionRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn một nguyên liệu")]
        public Guid IngredientId { get; set; }

        [Required(ErrorMessage = "Loại hạn chế không được để trống")]
        public required string Type { get; set; }

        [MaxLength(255, ErrorMessage = "Ghi chú không được vượt quá 255 ký tự")]
        public string? Notes { get; set; }

        public DateTime? ExpiredAtUtc { get; set; }
    }
}
