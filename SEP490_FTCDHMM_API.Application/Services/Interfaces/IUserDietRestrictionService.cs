using SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserDietRestrictionService
    {
        Task<IEnumerable<UserDietRestrictionResponse>> GetUserDietRestrictionsAsync(Guid userId, UserDietRestrictionFilterRequest request);
        Task CreateIngredientRestrictionAsync(Guid userId, CreateIngredientRestrictionRequest request);
        Task CreateIngredientCategoryRestrictionAsync(Guid userId, CreateIngredientCategoryRestrictionRequest request);
        Task DeleteRestrictionAsync(Guid userId, Guid restrictionId);
    }
}
