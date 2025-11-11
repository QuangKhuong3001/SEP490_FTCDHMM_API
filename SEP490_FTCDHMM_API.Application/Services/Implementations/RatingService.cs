using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;
        private readonly IRealtimeNotifier _notifier;
        private readonly IRecipeRepository _recipeRepository;

        public RatingService(IRatingRepository ratingRepository, IMapper mapper, IRealtimeNotifier notifier, IRecipeRepository recipeRepository)
        {
            _ratingRepository = ratingRepository;
            _mapper = mapper;
            _notifier = notifier;
            _recipeRepository = recipeRepository;
        }

        public async Task AddOrUpdate(Guid userId, Guid recipeId, RatingRequest request)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            var existingRating = await _ratingRepository.GetLatestAsync(r => r.UserId == userId && r.RecipeId == recipeId);

            if (existingRating != null)
            {
                existingRating.Score = request.Score;
                await _ratingRepository.UpdateAsync(existingRating);
            }
            else
            {
                var rating = new Rating
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    Feedback = request.Feedback,
                    Score = request.Score,
                    CreatedAtUtc = DateTime.UtcNow
                };
                await _ratingRepository.AddAsync(rating);
                existingRating = rating;
            }

            var allRatings = await _ratingRepository.GetAllAsync(r => r.RecipeId == recipeId);

            var avg = allRatings.Average(r => r.Score);
            recipe.Rating = avg;

            await _recipeRepository.UpdateAsync(recipe);

            await _notifier.SendRatingUpdateAsync(recipeId, existingRating);
        }

        public async Task Delete(Guid userId, Guid ratingId)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId);
            if (rating == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Đánh giá không tồn tại");

            if (rating.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN, "Không có quyền xóa đánh giá này");

            await _ratingRepository.DeleteAsync(rating);

            await _notifier.SendRatingDeletedAsync(rating.RecipeId, ratingId);
        }
    }
}
