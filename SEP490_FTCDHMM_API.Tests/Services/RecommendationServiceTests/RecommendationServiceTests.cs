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
        private readonly Mock<IUserRepository> _userRepo;
        private readonly Mock<IRecipeRepository> _recipeRepo;
        private readonly Mock<IRecipeScoringSystem> _scoring;
        private readonly Mock<IMapper> _mapper;
        private readonly RecommendationService _service;
        private readonly Mock<ICacheService> _cache;

        public RecommendationServiceTests()
        {
            _userRepo = new Mock<IUserRepository>();
            _recipeRepo = new Mock<IRecipeRepository>();
            _scoring = new Mock<IRecipeScoringSystem>();
            _mapper = new Mock<IMapper>();
            _cache = new Mock<ICacheService>();

            _service = new RecommendationService(
                _userRepo.Object,
                _recipeRepo.Object,
                _mapper.Object,
                _cache.Object,
                _scoring.Object
            );
        }

        [Fact]
        public async Task RecommendRecipesAsync_NoRecipes_ReturnsEmpty()
        {
            var userId = Guid.NewGuid();

            _userRepo.Setup(x => x.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new AppUser());

            _recipeRepo.Setup(x => x.GetActiveRecentRecipesAsync())
                .ReturnsAsync(new List<Recipe>());

            var result = await _service.RecommendRecipesAsync(
                userId,
                new PaginationParams { PageNumber = 1, PageSize = 10 }
            );

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task RecommendRecipesAsync_Sorts_ByScore_Correctly()
        {
            var user = new AppUser { Id = Guid.NewGuid() };

            _userRepo.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            var r1 = new Recipe { Id = Guid.NewGuid(), UpdatedAtUtc = DateTime.UtcNow.AddDays(-1) };
            var r2 = new Recipe { Id = Guid.NewGuid(), UpdatedAtUtc = DateTime.UtcNow.AddDays(-2) };
            var r3 = new Recipe { Id = Guid.NewGuid(), UpdatedAtUtc = DateTime.UtcNow.AddDays(-3) };

            var list = new List<Recipe> { r1, r2, r3 };

            _recipeRepo.Setup(x => x.GetActiveRecentRecipesAsync())
                .ReturnsAsync(list);

            _scoring.Setup(x => x.CalculateFinalScore(user, r1)).Returns(30);
            _scoring.Setup(x => x.CalculateFinalScore(user, r2)).Returns(50);
            _scoring.Setup(x => x.CalculateFinalScore(user, r3)).Returns(10);

            _mapper.Setup(x => x.Map<List<RecipeRankResponse>>(It.IsAny<List<Recipe>>()))
                .Returns((List<Recipe> src) =>
                    src.Select(r => new RecipeRankResponse { Id = r.Id }).ToList()
                );

            var result = await _service.RecommendRecipesAsync(
                user.Id,
                new PaginationParams { PageNumber = 1, PageSize = 10 }
            );

            var items = result.Items.ToList();

            Assert.Equal(50, items[0].Score);
            Assert.Equal(30, items[1].Score);
            Assert.Equal(10, items[2].Score);
        }

        [Fact]
        public async Task RecommendRecipesAsync_Pagination_Works()
        {
            var user = new AppUser { Id = Guid.NewGuid() };

            _userRepo.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            var r1 = new Recipe { Id = Guid.NewGuid(), UpdatedAtUtc = DateTime.UtcNow };
            var r2 = new Recipe { Id = Guid.NewGuid(), UpdatedAtUtc = DateTime.UtcNow };

            _recipeRepo.Setup(x => x.GetActiveRecentRecipesAsync())
                .ReturnsAsync(new List<Recipe> { r1, r2 });

            _scoring.Setup(x => x.CalculateFinalScore(user, It.IsAny<Recipe>())).Returns(10);

            _mapper.Setup(x => x.Map<List<RecipeRankResponse>>(It.IsAny<List<Recipe>>()))
                .Returns((List<Recipe> src) =>
                    src.Select(r => new RecipeRankResponse { Id = r.Id }).ToList()
                );

            var result = await _service.RecommendRecipesAsync(
                user.Id,
                new PaginationParams { PageNumber = 1, PageSize = 1 }
            );

            Assert.Single(result.Items);
            Assert.Equal(2, result.TotalCount);
        }
    }

}