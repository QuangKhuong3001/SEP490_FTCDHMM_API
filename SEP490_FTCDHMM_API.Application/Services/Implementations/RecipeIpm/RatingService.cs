using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeIpm
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IRealtimeNotifier _notifier;
        private readonly IRecipeBehaviorService _recipeBehaviorService;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public RatingService(
            IRatingRepository ratingRepository,
            IRealtimeNotifier notifier,
            IRecipeBehaviorService recipeBehaviorService,
            IRecipeRepository recipeRepository,
            IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _notifier = notifier;
            _recipeBehaviorService = recipeBehaviorService;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        public async Task AddOrUpdate(Guid userId, Guid recipeId, RatingRequest request)
        {
            if (request.Score < 4 && request.Feedback.IsNullOrEmpty())
                throw new AppException(AppResponseCode.INVALID_ACTION, "Nhận xét là bắt buộc khi đánh giá từ 3 sao đổ xuống");

            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            var existingRating = await _ratingRepository.GetLatestAsync(
                orderByDescendingKeySelector: r => r.CreatedAtUtc,
                predicate: r => r.UserId == userId
                                && r.RecipeId == recipeId);

            if (existingRating != null)
            {
                existingRating.Score = request.Score;
                existingRating.Feedback = request.Feedback;
                await _ratingRepository.UpdateAsync(existingRating);
                await _recipeBehaviorService.RecordRatingAsync(userId, recipeId, request.Score);
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
                await _recipeBehaviorService.RecordUpdateRatingAsync(userId, recipeId, request.Score);
                existingRating = rating;
            }

            var allRatings = await _ratingRepository.GetAllAsync(r => r.RecipeId == recipeId);

            var avg = allRatings.Average(r => r.Score);

            recipe.AvgRating = avg;
            recipe.RatingCount = allRatings.Count;

            await _recipeRepository.UpdateAsync(recipe);

            var ratingResponse = _mapper.Map<RatingDetailsResponse>(existingRating);
            await _notifier.SendRatingUpdateAsync(recipeId, ratingResponse);
        }

        public async Task Delete(Guid userId, Guid ratingId)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId);
            if (rating == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Đánh giá không tồn tại");

            if (rating.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN, "Không có quyền xóa đánh giá này");

            await _ratingRepository.DeleteAsync(rating);
        }
    }
}
