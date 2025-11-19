using System.Text.Json.Serialization;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos
{
    public class RecipePaginationParams
    {
        public int PageNumber { get; set; }
        [JsonIgnore]
        public int PageSize { get; } = 12;
    }
}
