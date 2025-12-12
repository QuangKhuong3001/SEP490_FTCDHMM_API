using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeQueryServiceTests
{
    public class GetRecipeHistoriesAsyncTests : RecipeQueryServiceTestBase
    {
        public GetRecipeHistoriesAsyncTests()
        {
            MapperMock
                .Setup(m => m.Map<IEnumerable<RecipeResponse>>(It.IsAny<IEnumerable<Recipe>>()))
                .Returns((IEnumerable<Recipe> src) =>
                    src.Select(r => new RecipeResponse { Id = r.Id }).ToList());
        }

        [Fact]
        public async Task Histories_ShouldReturnPagedResult()
        {
            var userId = NewId();

            var v1 = new RecipeUserView
            {
                UserId = userId,
                ViewedAtUtc = DateTime.UtcNow,
                Recipe = new Recipe
                {
                    Id = NewId(),
                    Name = "A",
                    Status = RecipeStatus.Posted,
                    Author = new AppUser { Id = NewId() },
                    Image = new Image { Id = NewId() }
                }
            };

            var v2 = new RecipeUserView
            {
                UserId = userId,
                ViewedAtUtc = DateTime.UtcNow.AddMinutes(-10),
                Recipe = new Recipe
                {
                    Id = NewId(),
                    Name = "B",
                    Status = RecipeStatus.Posted,
                    Author = new AppUser { Id = NewId() },
                    Image = new Image { Id = NewId() }
                }
            };

            var views = new List<RecipeUserView> { v1, v2 };

            UserRecipeViewRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<RecipeUserView, bool>>>(),
                    It.IsAny<Func<IQueryable<RecipeUserView>, IOrderedQueryable<RecipeUserView>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<RecipeUserView>, IQueryable<RecipeUserView>>?>()
                ))
                .ReturnsAsync((views, views.Count));

            var req = new RecipePaginationParams { PageNumber = 1 };

            var result = await Sut.GetRecipeHistoriesAsync(userId, req);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public async Task Histories_ShouldApplyPaginationCorrectly()
        {
            var userId = NewId();

            var list = Enumerable.Range(1, 15)
                .Select(i => new RecipeUserView
                {
                    UserId = userId,
                    ViewedAtUtc = DateTime.UtcNow.AddMinutes(-i),
                    Recipe = new Recipe
                    {
                        Id = Guid.NewGuid(),
                        Status = RecipeStatus.Posted,
                        Name = "R" + i,
                        Author = new AppUser { Id = Guid.NewGuid() },
                        Image = new Image { Id = Guid.NewGuid() }
                    }
                })
                .ToList();

            var pageItems = list.Skip(5).Take(5).ToList();

            UserRecipeViewRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    2,
                    12,
                    It.IsAny<Expression<Func<RecipeUserView, bool>>>(),
                    It.IsAny<Func<IQueryable<RecipeUserView>, IOrderedQueryable<RecipeUserView>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<RecipeUserView>, IQueryable<RecipeUserView>>?>()
                ))
                .ReturnsAsync((pageItems, list.Count));

            var req = new RecipePaginationParams
            {
                PageNumber = 2,
            };

            var result = await Sut.GetRecipeHistoriesAsync(userId, req);

            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Items.Count());
        }

        [Fact]
        public async Task Histories_ShouldReturnEmpty_WhenNoHistory()
        {
            var userId = NewId();

            UserRecipeViewRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<RecipeUserView, bool>>>(),
                    It.IsAny<Func<IQueryable<RecipeUserView>, IOrderedQueryable<RecipeUserView>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<RecipeUserView>, IQueryable<RecipeUserView>>?>()
                ))
                .ReturnsAsync((new List<RecipeUserView>(), 0));

            MapperMock
                .Setup(m => m.Map<IEnumerable<RecipeResponse>>(It.IsAny<IEnumerable<Recipe>>()))
                .Returns(new List<RecipeResponse>());

            var req = new RecipePaginationParams { PageNumber = 1 };

            var result = await Sut.GetRecipeHistoriesAsync(userId, req);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }
    }
}
