using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.Common
{
    public class PaginationParams
    {
        [Range(1, int.MaxValue, ErrorMessage = "Vị trí trang không được để trống")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Độ lớn trang phải phải lớn hơn 1")]
        public int PageSize { get; set; } = 12;
    }
}
