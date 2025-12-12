using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeQueryServiceTests
{
    public class GetSavedRecipesAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task GetSaved_ShouldReturnPagedResult()
        {
            var userId = NewId();

            var recipe1 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                NormalizedName = "a",
                Author = new AppUser()
            };

            var recipe2 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                NormalizedName = "b",
                Author = new AppUser()
            };

            var savedItems = new List<RecipeUserSave>
            {
                new() { UserId = userId, Recipe = recipe1, CreatedAtUtc = DateTime.UtcNow },
                new() { UserId = userId, Recipe = recipe2, CreatedAtUtc = DateTime.UtcNow }
            };

            UserSaveRecipeRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<RecipeUserSave, bool>>>(),
                    It.IsAny<Func<IQueryable<RecipeUserSave>, IOrderedQueryable<RecipeUserSave>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<RecipeUserSave>, IQueryable<RecipeUserSave>>?>()
                ))
                .ReturnsAsync((savedItems, savedItems.Count));

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeResponse>
                {
                    new() { Id = recipe1.Id },
                    new() { Id = recipe2.Id }
                });

            var req = new SaveRecipeFilterRequest
            {
                PaginationParams = new RecipePaginationParams { PageNumber = 1 }
            };

            var result = await Sut.GetSavedRecipesAsync(userId, req);

            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);

            UserSaveRecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetSaved_ShouldReturnEmpty_WhenNoSavedRecipes()
        {
            var userId = NewId();

            UserSaveRecipeRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<RecipeUserSave, bool>>>(),
                    It.IsAny<Func<IQueryable<RecipeUserSave>, IOrderedQueryable<RecipeUserSave>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<RecipeUserSave>, IQueryable<RecipeUserSave>>?>()
                ))
                .ReturnsAsync((new List<RecipeUserSave>(), 0));

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeResponse>());

            var req = new SaveRecipeFilterRequest
            {
                PaginationParams = new RecipePaginationParams { PageNumber = 1 }
            };

            var result = await Sut.GetSavedRecipesAsync(userId, req);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);

            UserSaveRecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
