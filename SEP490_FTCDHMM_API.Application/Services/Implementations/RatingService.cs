using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.Realtime;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;
        private readonly IRealtimeNotifier _notifier;

        public RatingService(IRatingRepository ratingRepository, IMapper mapper, IRealtimeNotifier notifier)
        {
            _ratingRepository = ratingRepository;
            _mapper = mapper;
            _notifier = notifier;
        }

        public async Task<RatingResponse> AddOrUpdateAsync(Guid userId, Guid recipeId, CreateRatingRequest request)
        {

            var existingRatings = await _ratingRepository.GetAllAsync(r => r.UserId == userId && r.RecipeId == recipeId);
            var existing = existingRatings.FirstOrDefault();

            if (existing != null)
            {
                existing.Score = request.Score;
                await _ratingRepository.UpdateAsync(existing);
            }
            else
            {
                var rating = new Rating
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    Score = request.Score,
                    CreatedAtUtc = DateTime.UtcNow
                };
                await _ratingRepository.AddAsync(rating);
                existing = rating;
            }


            var allRatings = await _ratingRepository.GetAllAsync(r => r.RecipeId == recipeId);
            var avg = allRatings.Any() ? allRatings.Average(r => r.Score) : 0;


            var saved = await _ratingRepository.GetByIdAsync(existing.Id, r => r.User);


            await _notifier.SendRatingUpdateAsync(recipeId, avg);

            return _mapper.Map<RatingResponse>(saved);
        }

        public async Task<double> GetAverageRatingAsync(Guid recipeId)
        {
            var ratings = await _ratingRepository.GetAllAsync(r => r.RecipeId == recipeId);
            return ratings.Any() ? ratings.Average(r => r.Score) : 0;
        }
    }
}
