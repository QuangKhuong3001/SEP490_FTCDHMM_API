using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeQueryServiceTests
{
    public class GetRecipesByUserIdAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task GetByUserId_ShouldReturnPagedResult()
        {
            var userId = NewId();

            var r1 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                AuthorId = userId
            };

            var r2 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                AuthorId = userId
            };

            var recipes = new List<Recipe> { r1, r2 };

            RecipeRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>()
                ))
                .ReturnsAsync((recipes, recipes.Count));

            MapperMock
                .Setup(m => m.Map<List<MyRecipeResponse>>(recipes))
                .Returns(recipes.Select(r => new MyRecipeResponse { Id = r.Id }).ToList());

            var req = new RecipePaginationParams { PageNumber = 1 };

            var result = await Sut.GetRecipesByUserIdAsync(userId, req);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.Contains(result.Items, x => x.Id == r1.Id);
            Assert.Contains(result.Items, x => x.Id == r2.Id);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetByUserId_ShouldApplyPaginationCorrectly()
        {
            var userId = NewId();

            var all = Enumerable.Range(1, 12)
                .Select(i => new Recipe
                {
                    Id = Guid.NewGuid(),
                    Status = RecipeStatus.Posted,
                    AuthorId = userId
                })
                .ToList();

            var pageItems = all.Skip(5).Take(5).ToList();

            RecipeRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    2,
                    12,
                    It.IsAny<Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>()
                ))
                .ReturnsAsync((pageItems, all.Count));

            MapperMock
                .Setup(m => m.Map<List<MyRecipeResponse>>(pageItems))
                .Returns(pageItems.Select(x => new MyRecipeResponse { Id = x.Id }).ToList());

            var req = new RecipePaginationParams { PageNumber = 2 };

            var res = await Sut.GetRecipesByUserIdAsync(userId, req);

            Assert.Equal(12, res.TotalCount);
            Assert.Equal(5, res.Items.Count());

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnEmpty_WhenNoRecipes()
        {
            var userId = NewId();

            RecipeRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>()
                ))
                .ReturnsAsync((new List<Recipe>(), 0));

            MapperMock
                .Setup(m => m.Map<List<MyRecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<MyRecipeResponse>());

            var req = new RecipePaginationParams { PageNumber = 1 };

            var result = await Sut.GetRecipesByUserIdAsync(userId, req);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
