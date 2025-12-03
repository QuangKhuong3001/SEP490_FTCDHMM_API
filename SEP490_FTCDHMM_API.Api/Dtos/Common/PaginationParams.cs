using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.Common
{
    public class PaginationParams
    {
        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn hoặc bằng 1")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Độ lớn trang phải phải lớn hơn 1")]
        public int PageSize { get; set; } = 12;
    }
}
