namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos
{
    public class CreateIngredientCategoryRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
