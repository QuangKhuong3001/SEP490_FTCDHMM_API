using System.Linq.Expressions;
using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.MealDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services
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
        private readonly Mock<IMealSlotRepository> _mealSlotRepo = new();
        private readonly Mock<IMealTargetProvider> _mealTargetProvider = new();
        private readonly Mock<IMealNutritionCalculator> _mealNutritionCalculator = new();
        private readonly Mock<IMealGapCalculator> _mealGapCalculator = new();
        private readonly Mock<IMealCompletionRecommender> _mealCompletionRecommender = new();

        private RecommendationService CreateService()
            => new RecommendationService(
                _userRepo.Object,
                _recipeRepo.Object,
                _mapper.Object,
                _cache.Object,
                _ratingRepo.Object,
                _viewRepo.Object,
                _commentRepo.Object,
                _saveRepo.Object,
                _mealTargetProvider.Object,
                _mealNutritionCalculator.Object,
                _mealGapCalculator.Object,
                _mealCompletionRecommender.Object,
                _mealSlotRepo.Object,
                _scoring.Object
            );

        private static UserMealSlot CreateMealSlot(Guid userId)
        {
            return new UserMealSlot
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Lunch",
                EnergyPercent = 0.3m,
                OrderIndex = 2
            };
        }

        [Fact]
        public async Task ComputedRecommendRecipesAsync_ReturnsEmpty_WhenClusterMissing()
        {
            var userId = Guid.NewGuid();

            _cache.Setup(x => x.GetAsync<int?>(It.IsAny<string>()))
                .ReturnsAsync((int?)null);

            var service = CreateService();
            var result = await service.ComputedRecommendRecipesAsync(userId);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task ComputedRecommendRecipesAsync_NullifiesScore_WhenCacheHit()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            _cache.Setup(x => x.GetAsync<int?>(It.IsAny<string>()))
                .ReturnsAsync(1);

            _cache.Setup(x => x.GetAsync<List<RecipeRankResponse>>(It.IsAny<string>()))
                .ReturnsAsync(new List<RecipeRankResponse>
                {
                    new() { Id = recipeId, Score = 10 }
                });

            _recipeRepo.Setup(x => x.GetAllAsync(
                    It.IsAny<Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(new List<Recipe>
                {
                    new Recipe { Id = recipeId }
                });

            _mapper.Setup(x => x.Map<RecipeRankResponse>(It.IsAny<Recipe>()))
                .Returns(new RecipeRankResponse { Id = recipeId });

            var service = CreateService();
            var result = await service.ComputedRecommendRecipesAsync(userId);

            Assert.Single(result.Items);
            Assert.Null(result.Items.First().Score);
        }

        [Fact]
        public async Task RecommendRecipesAsync_ReturnsEmpty_WhenNoSnapshots()
        {
            var userId = Guid.NewGuid();

            _cache.Setup(x => x.GetAsync<List<RankedRecipe>>(It.IsAny<string>()))
                .ReturnsAsync((List<RankedRecipe>?)null);

            _recipeRepo.Setup(x => x.GetRecipesForScoringAsync())
                .ReturnsAsync(new List<RecipeScoringSnapshot>());

            var service = CreateService();

            var result = await service.RecommendRecipesAsync(
                userId,
                new PaginationParams { PageNumber = 1, PageSize = 10 });

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task AnalyzeMealAsync_ReturnsZero_WhenUserHasNoTdee()
        {
            var userId = Guid.NewGuid();
            var slot = CreateMealSlot(userId);

            _mealSlotRepo.Setup(x => x.GetByIdAsync(slot.Id, null))
                .ReturnsAsync(slot);

            _userRepo.Setup(x => x.GetAllAsync(
                    It.IsAny<Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new List<AppUser>
                {
                    new AppUser
                    {
                        Id = userId,
                        HealthMetrics = new List<UserHealthMetric>()
                    }
                });

            var service = CreateService();

            var result = await service.AnalyzeMealAsync(
                userId,
                new MealAnalyzeRequest
                {
                    MealSlotId = slot.Id
                });

            Assert.Equal(0, result.TargetCalories);
            Assert.Equal(0, result.CurrentCalories);
            Assert.Equal(0, result.RemainingCalories);
        }

        [Fact]
        public async Task AnalyzeMealAsync_ReturnsSuggestions_WhenValid()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var slot = CreateMealSlot(userId);

            _mealSlotRepo.Setup(x => x.GetByIdAsync(slot.Id, null))
                .ReturnsAsync(slot);

            _userRepo.Setup(x => x.GetAllAsync(
                    It.IsAny<Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new List<AppUser>
                {
                    new AppUser
                    {
                        Id = userId,
                        HealthMetrics = new List<UserHealthMetric>
                        {
                            new UserHealthMetric { TDEE = 2000, RecordedAt = DateTime.UtcNow }
                        },
                        UserHealthGoals = new List<UserHealthGoal>(),
                        DietRestrictions = new List<UserDietRestriction>()
                    }
                });

            _recipeRepo.Setup(x => x.GetRecipesForScoringAsync())
                .ReturnsAsync(new List<RecipeScoringSnapshot>
                {
                    new RecipeScoringSnapshot { Id = recipeId }
                });

            _mealTargetProvider
                .Setup(x => x.BuildMealTarget(
                    It.IsAny<double>(),
                    It.IsAny<UserMealSlot>(),
                    It.IsAny<IReadOnlyList<NutrientTarget>>()))
                .Returns(new MealTarget(
                    600,
                    new List<NutrientTarget>()
                ));

            _mealNutritionCalculator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<RecipeScoringSnapshot>>()))
                .Returns(new MealNutritionState(
                    200,
                    new Dictionary<Guid, decimal>()
                ));

            _mealGapCalculator
                .Setup(x => x.Calculate(
                    It.IsAny<MealTarget>(),
                    It.IsAny<MealNutritionState>()))
                .Returns(new MealGap(
                    400,
                    new List<NutrientGap>()
                ));

            _mealCompletionRecommender
                .Setup(x => x.Recommend(
                    It.IsAny<RecommendationUserContext>(),
                    It.IsAny<MealTarget>(),
                    It.IsAny<MealNutritionState>(),
                    It.IsAny<MealGap>(),
                    It.IsAny<IEnumerable<RecipeScoringSnapshot>>(),
                    It.IsAny<HashSet<Guid>>(),
                    It.IsAny<int>()))
                .Returns(new[]
                {
                    (
                        new RecipeScoringSnapshot { Id = recipeId },
                        0.9
                    )
                });

            _recipeRepo.Setup(x => x.GetAllAsync(
                    It.IsAny<Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(new List<Recipe>
                {
                    new Recipe { Id = recipeId }
                });

            _mapper.Setup(x => x.Map<List<RecipeRankResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeRankResponse>
                {
                    new RecipeRankResponse { Id = recipeId }
                });

            var service = CreateService();

            var result = await service.AnalyzeMealAsync(
                userId,
                new MealAnalyzeRequest
                {
                    MealSlotId = slot.Id,
                    SuggestionLimit = 5
                });

            Assert.Single(result.Suggestions);
            Assert.Equal(600, result.TargetCalories);
            Assert.Equal(200, result.CurrentCalories);
            Assert.Equal(400, result.RemainingCalories);
        }
    }
}
