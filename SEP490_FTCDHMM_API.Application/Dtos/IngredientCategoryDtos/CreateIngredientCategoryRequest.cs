namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos
{
    public class CreateIngredientCategoryRequest
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
