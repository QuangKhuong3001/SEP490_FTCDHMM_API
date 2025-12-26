using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.MealDtos
{
    public class MealSlotRequest
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Tên phải từ 1 đến 255 ký tự")]
        public string Name { get; set; } = null!;

        [Range(1, 100, ErrorMessage = "Phần trăm năng lượng phải từ 1 đến 100")]
        public decimal EnergyPercent { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Chỉ số thứ tự phải là số nguyên dương")]
        public int OrderIndex { get; set; }
    }
}
