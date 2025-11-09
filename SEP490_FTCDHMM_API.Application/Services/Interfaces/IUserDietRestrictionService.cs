using SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserDietRestrictionService
    {
        Task<IEnumerable<UserDietRestrictionResponse>> GetUserDietRestrictionsAsync(Guid userId, UserDietRestrictionFilterRequest request);
        Task CreateIngredientRestriction(Guid userId, CreateIngredientRestrictionRequest request);
        Task CreateIngredientCategoryRestriction(Guid userId, CreateIngredientCategoryRestrictionRequest request);
        Task DeleteRestriction(Guid userId, Guid restrictionId);
    }
}
