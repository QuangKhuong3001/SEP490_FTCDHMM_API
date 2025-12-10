using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IRatingService
    {
        Task AddOrUpdateRatingAsync(Guid userId, Guid recipeId, RatingRequest request);
        Task DeleteRatingAsync(Guid userId, Guid ratingId);
        Task DeleteRatingByManagerAsync(Guid ratingId);
    }
}
