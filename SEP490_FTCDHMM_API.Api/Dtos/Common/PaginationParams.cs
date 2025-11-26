using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.Common
{
    public class PaginationParams
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Vị trí trang không được để trống")]
        public int PageNumber { get; set; } = 1;

        [Required]
        [AllowedValues([10, 20, 50], ErrorMessage = "Độ lớn trang phải là 10, 20 hoặc 50")]
        public int PageSize { get; set; } = 10;
    }
}
