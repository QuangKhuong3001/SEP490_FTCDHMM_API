using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos
{
    public class NutrientRequest
    {
        [Required(ErrorMessage = "Chất dinh dưỡng không được để trống")]
        public Guid NutrientId { get; set; }

        [Required(ErrorMessage = "Giá trị không được để trống.")]
        [Range(0, 9999999.999, ErrorMessage = "Giá trị phải từ 0 đến 9999999.999")]
        public decimal Value { get; set; }
    }
}
