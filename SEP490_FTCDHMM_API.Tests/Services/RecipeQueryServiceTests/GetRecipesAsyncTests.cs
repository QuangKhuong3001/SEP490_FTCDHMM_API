using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Specifications;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeQueryServiceTests
{
    public class GetRecipesAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task GetRecipes_ShouldReturnEmpty_WhenNoRankingSource()
        {
            CacheServiceMock
                .Setup(c => c.GetAsync<PagedResult<RecipeResponse>>(It.IsAny<string>()))
                .ReturnsAsync((PagedResult<RecipeResponse>?)null);

            CacheServiceMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<PagedResult<RecipeResponse>>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            RecipeRepositoryMock
                .Setup(r => r.GetRecipesForRankingAsync(It.IsAny<RecipeBasicFilterSpec>()))
                .ReturnsAsync(new List<RecipeRankSource>());

            RecipeRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()
                ))
                .ReturnsAsync(new List<Recipe>());

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeResponse>());

            var res = await Sut.GetRecipesAsync(new RecipeFilterRequest());

            Assert.Empty(res.Items);
            Assert.Equal(0, res.TotalCount);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetRecipes_ShouldApplyPagingCorrectly()
        {
            CacheServiceMock
                .Setup(c => c.GetAsync<PagedResult<RecipeResponse>>(It.IsAny<string>()))
                .ReturnsAsync((PagedResult<RecipeResponse>?)null);

            CacheServiceMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<PagedResult<RecipeResponse>>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            var sources = Enumerable.Range(1, 20)
                .Select(i => new RecipeRankSource
                {
                    RecipeId = NewId(),
                    UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-i),
                    IngredientIds = new List<Guid>()
                })
                .ToList();

            RecipeRepositoryMock
                .Setup(r => r.GetRecipesForRankingAsync(It.IsAny<RecipeBasicFilterSpec>()))
                .ReturnsAsync(sources);

            RecipeRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(sources
                    .Skip(8)
                    .Take(8)
                    .Select(s => new Recipe
                    {
                        Id = s.RecipeId,
                        RecipeIngredients = new List<RecipeIngredient>()
                    })
                    .ToList());

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns((List<Recipe> src) =>
                    src.Select(r => new RecipeResponse { Id = r.Id }).ToList());

            var req = new RecipeFilterRequest
            {
                PaginationParams = new RecipePaginationParams
                {
                    PageNumber = 2
                }
            };

            var res = await Sut.GetRecipesAsync(req);

            Assert.Equal(4, res.Items.Count());
            Assert.Equal(20, res.TotalCount);
        }

        [Fact]
        public async Task GetRecipes_ShouldPreserveRankingOrder()
        {
            CacheServiceMock
                .Setup(c => c.GetAsync<PagedResult<RecipeResponse>>(It.IsAny<string>()))
                .ReturnsAsync((PagedResult<RecipeResponse>?)null);

            CacheServiceMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<PagedResult<RecipeResponse>>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            var r1 = NewId();
            var r2 = NewId();

            RecipeRepositoryMock
                .Setup(r => r.GetRecipesForRankingAsync(It.IsAny<RecipeBasicFilterSpec>()))
                .ReturnsAsync(new List<RecipeRankSource>
                {
                    new RecipeRankSource
                    {
                        RecipeId = r2,
                        UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-1),
                        IngredientIds = new List<Guid>()
                    },
                    new RecipeRankSource
                    {
                        RecipeId = r1,
                        UpdatedAtUtc = DateTime.UtcNow,
                        IngredientIds = new List<Guid>()
                    }
                });

            RecipeRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(new List<Recipe>
                {
                    new Recipe { Id = r1 },
                    new Recipe { Id = r2 }
                });

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns((List<Recipe> src) =>
                    src.Select(r => new RecipeResponse { Id = r.Id }).ToList());

            var req = new RecipeFilterRequest
            {
                PaginationParams = new RecipePaginationParams()
            };

            var res = await Sut.GetRecipesAsync(req);

            Assert.Equal(r1, res.Items.ElementAt(0).Id);
            Assert.Equal(r2, res.Items.ElementAt(1).Id);

        }

        [Fact]
        public async Task GetRecipes_ShouldReturnFromCache_WhenCacheHit()
        {
            var cached = new PagedResult<RecipeResponse>
            {
                Items = new List<RecipeResponse>
                {
                    new RecipeResponse { Id = NewId() }
                },
                TotalCount = 1
            };

            CacheServiceMock
                .Setup(c => c.GetAsync<PagedResult<RecipeResponse>>(It.IsAny<string>()))
                .ReturnsAsync(cached);

            var req = new RecipeFilterRequest
            {
                PaginationParams = new RecipePaginationParams()
            };

            var res = await Sut.GetRecipesAsync(req);

            Assert.Single(res.Items);
            RecipeRepositoryMock.Verify(
                r => r.GetRecipesForRankingAsync(It.IsAny<RecipeBasicFilterSpec>()),
                Times.Never);
        }
    }
}
