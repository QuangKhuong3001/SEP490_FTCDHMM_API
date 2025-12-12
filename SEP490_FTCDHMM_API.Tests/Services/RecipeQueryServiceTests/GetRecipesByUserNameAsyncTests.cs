using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeQueryServiceTests
{
    public class GetRecipesByUserNameAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task GetByUserName_ShouldThrow_WhenUserNotFound()
        {
            UserRepositoryMock
                .Setup(r => r.GetByUserNameAsync(It.IsAny<string>()))
                .ReturnsAsync((AppUser)null!);

            var req = new RecipePaginationParams();

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipesByUserNameAsync("abc", req));

            UserRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetByUserName_ShouldReturnPagedResult()
        {
            var user = new AppUser { Id = NewId(), UserName = "test" };

            UserRepositoryMock
                .Setup(r => r.GetByUserNameAsync("test"))
                .ReturnsAsync(user);

            var r1 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                AuthorId = user.Id,
                Name = "A",
                Image = new Image { Id = NewId() },
                RecipeIngredients = new List<RecipeIngredient>(),
                Labels = new List<Label>(),
                RecipeUserTags = new List<RecipeUserTag>(),
                CookingSteps = new List<CookingStep>()
            };

            var r2 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                AuthorId = user.Id,
                Name = "B",
                Image = new Image { Id = NewId() },
                RecipeIngredients = new List<RecipeIngredient>(),
                Labels = new List<Label>(),
                RecipeUserTags = new List<RecipeUserTag>(),
                CookingSteps = new List<CookingStep>()
            };

            var list = new List<Recipe> { r1, r2 };

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
                .ReturnsAsync((list, list.Count));

            MapperMock
                .Setup(m => m.Map<List<MyRecipeResponse>>(list))
                .Returns(list.Select(x => new MyRecipeResponse { Id = x.Id }).ToList());

            var req = new RecipePaginationParams
            {
                PageNumber = 1
            };

            var result = await Sut.GetRecipesByUserNameAsync("test", req);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.Contains(result.Items, x => x.Id == r1.Id);
            Assert.Contains(result.Items, x => x.Id == r2.Id);

            UserRepositoryMock.VerifyAll();
            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetByUserName_ShouldApplyPaginationCorrectly()
        {
            var user = new AppUser { Id = NewId(), UserName = "abc" };

            UserRepositoryMock
                .Setup(r => r.GetByUserNameAsync("abc"))
                .ReturnsAsync(user);

            var all = Enumerable.Range(1, 12)
                .Select(i => new Recipe
                {
                    Id = Guid.NewGuid(),
                    Status = RecipeStatus.Posted,
                    AuthorId = user.Id,
                    Name = "R" + i,
                    Image = new Image { Id = Guid.NewGuid() },
                    RecipeIngredients = new List<RecipeIngredient>(),
                    Labels = new List<Label>(),
                    RecipeUserTags = new List<RecipeUserTag>(),
                    CookingSteps = new List<CookingStep>()
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

            var req = new RecipePaginationParams
            {
                PageNumber = 2,
            };

            var result = await Sut.GetRecipesByUserNameAsync("abc", req);

            Assert.Equal(12, result.TotalCount);
            Assert.Equal(5, result.Items.Count());

            UserRepositoryMock.VerifyAll();
            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetByUserName_ShouldReturnEmpty_WhenNoRecipes()
        {
            var user = new AppUser { Id = NewId(), UserName = "x" };

            UserRepositoryMock
                .Setup(r => r.GetByUserNameAsync("x"))
                .ReturnsAsync(user);

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

            var req = new RecipePaginationParams
            {
                PageNumber = 1,
            };

            var result = await Sut.GetRecipesByUserNameAsync("x", req);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);

            UserRepositoryMock.VerifyAll();
            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
