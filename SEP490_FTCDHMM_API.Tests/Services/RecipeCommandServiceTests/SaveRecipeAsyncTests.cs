using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeCommandServiceTests
{
    public class SaveRecipeAsyncTests : RecipeCommandServiceTestBase
    {
        [Fact]
        public async Task SaveRecipeAsync_ShouldThrow_WhenRecipeNotFound()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(recipeId, null))
                .ReturnsAsync((Recipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.SaveRecipeAsync(userId, recipeId));
        }

        [Fact]
        public async Task SaveRecipeAsync_ShouldThrow_WhenAlreadySaved()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipe = CreateRecipe(recipeId);

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            UserSaveRecipeRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<UserSaveRecipe, bool>>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.SaveRecipeAsync(userId, recipeId));
        }

        [Fact]
        public async Task SaveRecipeAsync_ShouldSave()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipe = CreateRecipe(recipeId);

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            UserSaveRecipeRepositoryMock
                .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<UserSaveRecipe, bool>>>()))
                .ReturnsAsync(false);

            UserSaveRecipeRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<UserSaveRecipe>()))
                .ReturnsAsync((UserSaveRecipe u) => u);

            await Sut.SaveRecipeAsync(userId, recipeId);

            UserSaveRecipeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserSaveRecipe>()), Times.Once);
        }
    }
}
