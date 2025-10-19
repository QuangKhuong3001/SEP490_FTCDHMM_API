using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.Common
{
    public class PaginationParams
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0")]
        public int PageNumber { get; set; } = 1;

        [Required]
        [AllowedValues([10, 20, 50], ErrorMessage = "PageSize must be 10, 20, or 50")]
        public int PageSize { get; set; } = 10;
    }
}
