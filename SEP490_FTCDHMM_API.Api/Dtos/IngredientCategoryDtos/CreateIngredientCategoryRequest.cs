using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientCategoryDtos
{
    public class CreateIngredientCategoryRequest
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Thiếu tên ")]
        public string Name { get; set; } = string.Empty;
    }
}
