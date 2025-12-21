using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.RecommendationServiceTests
{
    public class RecommendationServiceTests
    {
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IRecipeRepository> _recipeRepo = new();
        private readonly Mock<IRecipeScoringSystem> _scoring = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<ICacheService> _cache = new();
        private readonly Mock<IRatingRepository> _ratingRepo = new();
        private readonly Mock<IUserRecipeViewRepository> _viewRepo = new();
        private readonly Mock<ICommentRepository> _commentRepo = new();
        private readonly Mock<IUserSaveRecipeRepository> _saveRepo = new();

        private RecommendationService CreateService()
        {
            return new RecommendationService(
                _userRepo.Object,
                _recipeRepo.Object,
                _mapper.Object,
                _cache.Object,
                _ratingRepo.Object,
                _viewRepo.Object,
                _commentRepo.Object,
                _saveRepo.Object,
                _scoring.Object
            );
        }

        [Fact]
        public async Task RecommendRecipesAsync_CacheHit_ReturnsCachedResult()
        {
            var userId = Guid.NewGuid();
            var cacheKey = $"recommend:user:{userId}:meal:dinner:page:1";

            var cached = new PagedResult<RecipeRankResponse>
            {
                Items = new List<RecipeRankResponse>
                {
                    new() { Id = Guid.NewGuid(), Score = 100 }
                },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };

            _cache.Setup(x => x.GetAsync<PagedResult<RecipeRankResponse>>(It.IsAny<string>()))
                .ReturnsAsync(cached);

            var service = CreateService();

            var result = await service.RecommendRecipesAsync(
                userId,
                new PaginationParams { PageNumber = 1, PageSize = 10 }
            );

            Assert.Single(result.Items);
            Assert.Equal(100, result.Items.First().Score);
        }

        [Fact]
        public async Task RecommendRecipesAsync_NoSnapshots_ReturnsEmpty()
        {
            _cache.Setup(x => x.GetAsync<PagedResult<RecipeRankResponse>>(It.IsAny<string>()))
                .ReturnsAsync((PagedResult<RecipeRankResponse>?)null);

            _recipeRepo.Setup(x => x.GetRecipesForScoringAsync())
                .ReturnsAsync(new List<RecipeScoringSnapshot>());

            var service = CreateService();

            var result = await service.RecommendRecipesAsync(
                Guid.NewGuid(),
                new PaginationParams { PageNumber = 1, PageSize = 10 }
            );

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task RecommendRecipesAsync_SortsByScoreDescending()
        {
            var userId = Guid.NewGuid();

            _cache.Setup(x => x.GetAsync<PagedResult<RecipeRankResponse>>(It.IsAny<string>()))
                .ReturnsAsync((PagedResult<RecipeRankResponse>?)null);

            var snapshots = new List<RecipeScoringSnapshot>
            {
                new() { Id = Guid.NewGuid(), UpdatedAtUtc = DateTime.UtcNow.AddDays(-1), LabelIds = new() },
                new() { Id = Guid.NewGuid(), UpdatedAtUtc = DateTime.UtcNow.AddDays(-2), LabelIds = new() },
                new() { Id = Guid.NewGuid(), UpdatedAtUtc = DateTime.UtcNow.AddDays(-3), LabelIds = new() }
            };

            _recipeRepo.Setup(x => x.GetRecipesForScoringAsync())
                .ReturnsAsync(snapshots);

            _userRepo.Setup(x => x.Query())
                .Returns(new List<AppUser>
                {
                    new()
                    {
                        Id = userId,
                        HealthMetrics = new List<UserHealthMetric>()
                    }
                }.AsQueryable());

            _ratingRepo.Setup(x => x.Query()).Returns(Enumerable.Empty<Rating>().AsQueryable());
            _commentRepo.Setup(x => x.Query()).Returns(Enumerable.Empty<Comment>().AsQueryable());
            _viewRepo.Setup(x => x.Query()).Returns(Enumerable.Empty<RecipeUserView>().AsQueryable());
            _saveRepo.Setup(x => x.Query()).Returns(Enumerable.Empty<RecipeUserSave>().AsQueryable());

            _scoring.Setup(x => x.CalculateFinalScore(It.IsAny<RecommendationUserContext>(), snapshots[0]))
                .Returns(10);
            _scoring.Setup(x => x.CalculateFinalScore(It.IsAny<RecommendationUserContext>(), snapshots[1]))
                .Returns(50);
            _scoring.Setup(x => x.CalculateFinalScore(It.IsAny<RecommendationUserContext>(), snapshots[2]))
                .Returns(30);

            var recipes = snapshots.Select(s => new Recipe { Id = s.Id }).ToList();

            _recipeRepo.Setup(x => x.Query())
                .Returns(recipes.AsQueryable());

            _mapper.Setup(x => x.Map<List<RecipeRankResponse>>(It.IsAny<List<Recipe>>()))
                .Returns((List<Recipe> src) =>
                    src.Select(r => new RecipeRankResponse { Id = r.Id }).ToList()
                );

            var service = CreateService();

            var result = await service.RecommendRecipesAsync(
                userId,
                new PaginationParams { PageNumber = 1, PageSize = 10 }
            );

            var items = result.Items.ToList();

            Assert.Equal(3, items.Count);
            Assert.Equal(50, items[0].Score);
            Assert.Equal(30, items[1].Score);
            Assert.Equal(10, items[2].Score);
        }
    }
}
