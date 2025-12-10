using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.IngredientServiceTests
{
    public class DeleteIngredientAsyncTests : IngredientServiceTestBase
    {
        [Fact]
        public async Task DeleteIngredient_ShouldThrowNotFound_WhenIngredientNotExists()
        {
            var id = NewId();

            IngredientRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<Ingredient>, IQueryable<Ingredient>>>()))
                .ReturnsAsync((Ingredient?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.DeleteIngredientAsync(id));

            Assert.Equal(AppResponseCode.NOT_FOUND, ex.ResponseCode);
            IngredientRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteIngredient_ShouldThrowInvalidAction_WhenUsedInRecipe()
        {
            var id = NewId();
            var ingredient = CreateIngredient(id);
            ingredient.RecipeIngredients.Add(new RecipeIngredient { IngredientId = ingredient.Id });

            IngredientRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<Ingredient>, IQueryable<Ingredient>>>()))
                .ReturnsAsync(ingredient);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.DeleteIngredientAsync(id));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);
            IngredientRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteIngredient_ShouldDeleteImageAndIngredient_WhenNotUsedInRecipe()
        {
            var id = NewId();
            var ingredient = CreateIngredient(id);

            IngredientRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<Ingredient>, IQueryable<Ingredient>>>()))
                .ReturnsAsync(ingredient);

            S3ImageServiceMock
                .Setup(s => s.DeleteImageAsync(ingredient.ImageId))
                .Returns(Task.CompletedTask);

            IngredientRepositoryMock
                .Setup(r => r.DeleteAsync(ingredient))
                .Returns(Task.CompletedTask);

            await Sut.DeleteIngredientAsync(id);

            IngredientRepositoryMock.VerifyAll();
            S3ImageServiceMock.VerifyAll();
        }
    }
}
