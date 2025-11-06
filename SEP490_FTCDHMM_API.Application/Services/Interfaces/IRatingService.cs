using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IRatingService
    {
        Task<RatingResponse> AddOrUpdateAsync(Guid userId, Guid recipeId, CreateRatingRequest request);
        Task<double> GetAverageRatingAsync(Guid recipeId);
    }
}
