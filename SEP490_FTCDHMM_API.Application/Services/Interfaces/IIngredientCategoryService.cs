using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IIngredientCategoryService
    {
        Task CreateCategory(CreateIngredientCategoryRequest request);
        Task DeleteCategory(Guid id);
        Task<PagedResult<IngredientCategoryResponse>> GetAllCategories(IngredientCategoryFilterRequest request);
        Task<IEnumerable<IngredientCategoryResponse>> GetAllCategories(IngredientCategorySearchDropboxRequest request);
    }
}
