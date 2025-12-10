using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IRealtimeNotifier _notifier;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public RatingService(
            IRatingRepository ratingRepository,
            IRealtimeNotifier notifier,
            IRecipeRepository recipeRepository,
            IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _notifier = notifier;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        public async Task AddOrUpdateRatingAsync(Guid userId, Guid recipeId, RatingRequest request)
        {
            if (request.Score <= 3 && (request.Feedback == null || request.Feedback.CleanDuplicateSpace().IsNullOrEmpty()))
                throw new AppException(AppResponseCode.INVALID_ACTION, "Nhận xét là bắt buộc khi đánh giá từ 3 sao đổ xuống");

            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (recipe == null || recipe.Status != RecipeStatus.Posted)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            var existingRating = await _ratingRepository.FirstOrDefaultAsync(
                orderByDescendingKeySelector: r => r.CreatedAtUtc,
                predicate: r => r.UserId == userId
                                && r.RecipeId == recipeId);

            if (existingRating != null)
            {
                existingRating.Score = request.Score;
                existingRating.Feedback = request.Feedback;
                existingRating.CreatedAtUtc = DateTime.UtcNow;
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

            await this.CalculateRating(recipeId);

            var ratingResponse = _mapper.Map<RatingDetailsResponse>(existingRating);
            await _notifier.SendRatingUpdateAsync(recipeId, ratingResponse);
        }

        public async Task DeleteRatingAsync(Guid userId, Guid ratingId)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId);
            if (rating == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Đánh giá không tồn tại");

            if (rating.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN, "Không có quyền xóa đánh giá này");

            await _ratingRepository.DeleteAsync(rating);

            await this.CalculateRating(rating.RecipeId);
            await _notifier.SendRatingDeletedAsync(rating.RecipeId, ratingId);
        }

        public async Task DeleteRatingByManagerAsync(Guid ratingId)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId);
            if (rating == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Đánh giá không tồn tại");

            await _ratingRepository.DeleteAsync(rating);

            await this.CalculateRating(rating.RecipeId);
            await _notifier.SendRatingDeletedAsync(rating.RecipeId, ratingId);
        }

        private async Task CalculateRating(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (recipe == null)
                return;

            var allRatings = await _ratingRepository.GetAllAsync(r => r.RecipeId == recipe.Id);
            var avg = allRatings.Any() ? allRatings.Average(r => r.Score) : 0;
            recipe.AvgRating = avg;
            recipe.RatingCount = allRatings.Count;

            await _recipeRepository.UpdateAsync(recipe);
        }
    }
}
