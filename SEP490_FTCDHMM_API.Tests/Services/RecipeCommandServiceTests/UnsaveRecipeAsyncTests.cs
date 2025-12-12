using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeCommandServiceTests
{
    public class UnsaveRecipeAsyncTests : RecipeCommandServiceTestBase
    {
        [Fact]
        public async Task UnsaveRecipeAsync_ShouldThrow_WhenRecipeNotFound()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(recipeId, null))
                .ReturnsAsync((Recipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UnsaveRecipeAsync(userId, recipeId));
        }

        [Fact]
        public async Task UnsaveRecipeAsync_ShouldThrow_WhenNotSaved()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipe = CreateRecipe(recipeId);

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            UserSaveRecipeRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<RecipeUserSave, bool>>>(), null))
                .ReturnsAsync(new List<RecipeUserSave>());

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UnsaveRecipeAsync(userId, recipeId));
        }

        [Fact]
        public async Task UnsaveRecipeAsync_ShouldUnsave()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipe = CreateRecipe(recipeId);

            var saved = new List<RecipeUserSave>
            {
                new RecipeUserSave { RecipeId = recipeId, UserId = userId }
            };

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            UserSaveRecipeRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<RecipeUserSave, bool>>>(), null))
                .ReturnsAsync(saved);

            UserSaveRecipeRepositoryMock
                .Setup(x => x.DeleteAsync(saved.First()))
                .Returns(Task.CompletedTask);

            await Sut.UnsaveRecipeAsync(userId, recipeId);

            UserSaveRecipeRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<RecipeUserSave>()), Times.Once);
        }
    }
}
