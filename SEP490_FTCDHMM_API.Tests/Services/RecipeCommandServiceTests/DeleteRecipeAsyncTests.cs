using Moq;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeCommandServiceTests
{
    public class DeleteRecipeAsyncTests : RecipeCommandServiceTestBase
    {
        [Fact]
        public async Task DeleteRecipeAsync_ShouldThrow_WhenRecipeNotFound()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(recipeId, null))
                .ReturnsAsync((Domain.Entities.Recipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteRecipeAsync(userId, recipeId));
        }

        [Fact]
        public async Task DeleteRecipeAsync_ShouldThrow_WhenNotOwner()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipe = CreateRecipe(recipeId);

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            RecipeValidationServiceMock
                .Setup(x => x.ValidateRecipeOwnerAsync(userId, recipe))
                .ThrowsAsync(new AppException(AppResponseCode.FORBIDDEN));

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteRecipeAsync(userId, recipeId));
        }

        [Fact]
        public async Task DeleteRecipeAsync_ShouldMarkAsDeleted()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipe = CreateRecipe(recipeId);

            RecipeValidationServiceMock.Setup(x => x.ValidateRecipeOwnerAsync(userId, recipe)).Returns(Task.CompletedTask);

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            RecipeRepositoryMock.Setup(x => x.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

            await Sut.DeleteRecipeAsync(userId, recipeId);

            RecipeRepositoryMock.Verify(x => x.UpdateAsync(recipe), Times.Once);
        }
    }
}
