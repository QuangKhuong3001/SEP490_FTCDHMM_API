using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.Common;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientDtos
{
    public class IngredientFilterRequest
    {
        public string? Keyword { get; set; }
        public List<Guid>? CategoryIds { get; set; }

        [ModelBinder(BinderType = typeof(SafeNullableDateTimeBinder))]
        public DateTime? UpdatedFrom { get; set; }

        [ModelBinder(BinderType = typeof(SafeNullableDateTimeBinder))]
        public DateTime? UpdatedTo { get; set; }
        public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    }

}
