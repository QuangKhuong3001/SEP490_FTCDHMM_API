using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation;

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
            => new(
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

        [Fact]
        public async Task ComputedCommendRecipesAsync_ReturnsEmpty_WhenClusterMissing()
        {
            var userId = Guid.NewGuid();

            _cache.Setup(x => x.GetAsync<int?>(It.IsAny<string>()))
                .ReturnsAsync((int?)null);

            var service = CreateService();
            var result = await service.ComputedCommendRecipesAsync(userId);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task ComputedCommendRecipesAsync_ReturnsEmpty_WhenCacheEmpty()
        {
            var userId = Guid.NewGuid();

            _cache.Setup(x => x.GetAsync<int?>(It.IsAny<string>()))
                .ReturnsAsync(1);

            _cache.Setup(x => x.GetAsync<List<RecipeRankResponse>>(It.IsAny<string>()))
                .ReturnsAsync(new List<RecipeRankResponse>());

            var service = CreateService();
            var result = await service.ComputedCommendRecipesAsync(userId);

            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task ComputedCommendRecipesAsync_NullifiesScore_WhenCacheHit()
        {
            var userId = Guid.NewGuid();

            var cached = new List<RecipeRankResponse>
            {
                new() { Id = Guid.NewGuid(), Score = 100 },
                new() { Id = Guid.NewGuid(), Score = 50 }
            };

            _cache.Setup(x => x.GetAsync<int?>(It.IsAny<string>()))
                .ReturnsAsync(1);

            _cache.Setup(x => x.GetAsync<List<RecipeRankResponse>>(It.IsAny<string>()))
                .ReturnsAsync(cached);

            var service = CreateService();
            var result = await service.ComputedCommendRecipesAsync(userId);

            Assert.All(result.Items, x => Assert.Null(x.Score));
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task RecommendRecipesAsync_ReturnsEmpty_WhenNoSnapshots()
        {
            var userId = Guid.NewGuid();

            _cache.Setup(x => x.GetAsync<PagedResult<RecipeRankResponse>>(It.IsAny<string>()))
                .ReturnsAsync((PagedResult<RecipeRankResponse>?)null);

            _recipeRepo.Setup(x => x.GetRecipesForScoringAsync())
                .ReturnsAsync(new List<RecipeScoringSnapshot>());

            var service = CreateService();
            var result = await service.RecommendRecipesAsync(
                userId,
                new PaginationParams { PageNumber = 1, PageSize = 10 }
            );

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }
    }
}
