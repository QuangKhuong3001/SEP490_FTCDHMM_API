using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IRatingService
    {
        Task AddOrUpdate(Guid userId, Guid recipeId, RatingRequest request);
        Task Delete(Guid userId, Guid ratingId);
    }
}
