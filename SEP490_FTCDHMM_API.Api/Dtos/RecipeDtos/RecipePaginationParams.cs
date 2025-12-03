using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos
{
    public class RecipePaginationParams
    {
        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn hoặc bằng 1")]
        public int PageNumber { get; set; } = 1;

        [JsonIgnore]
        public int PageSize { get; } = 12;
    }
}
