using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IIngredientCategoryService
    {
        Task CreateIngredientCategoryAsync(CreateIngredientCategoryRequest request);
        Task DeleteIngredientCategoryAsync(Guid id);
        Task<PagedResult<IngredientCategoryResponse>> GetAllIngredientCategoriesFilterAsync(IngredientCategoryFilterRequest request);
        Task<IEnumerable<IngredientCategoryResponse>> GetIngredientCategoriesAsync(IngredientCategorySearchDropboxRequest request);
    }
}
