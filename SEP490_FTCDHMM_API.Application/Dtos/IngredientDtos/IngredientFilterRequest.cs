namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos
{
    public class IngredientFilterRequest
    {
        public string? Keyword { get; set; }
        public List<Guid>? CategoryIds { get; set; }
        public DateTime? UpdatedFrom { get; set; }
        public DateTime? UpdatedTo { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
