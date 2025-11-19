using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos
{
    public class RecipePaginationParams
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0")]
        public int PageNumber { get; set; } = 1;

        [JsonIgnore]
        public int PageSize { get; } = 12;
    }
}
