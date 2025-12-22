using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.IngredientCategoryServiceTests
{
    public class DeleteIngredientCategoryAsyncTests : IngredientCategoryServiceTestBase
    {
        [Fact]
        public async Task Delete_ShouldThrow_WhenNotFound()
        {
            IngredientCateRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<IngredientCategory>, IQueryable<IngredientCategory>>>()))
                .ReturnsAsync((IngredientCategory)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.DeleteIngredientCategoryAsync(NewId()));
            IngredientCateRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenCategoryHasIngredients()
        {
            var cat = new IngredientCategory
            {
                Id = NewId(),
                Ingredients = new List<Ingredient> { new Ingredient() }
            };

            IngredientCateRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<IngredientCategory>, IQueryable<IngredientCategory>>>()))
                .ReturnsAsync(cat);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteIngredientCategoryAsync(cat.Id));

            IngredientCateRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Delete_ShouldHardDelete_WhenNoIngredients()
        {
            var cat = new IngredientCategory
            {
                Id = NewId(),
                Ingredients = new List<Ingredient>()
            };

            IngredientCateRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<IngredientCategory>, IQueryable<IngredientCategory>>>()))
                .ReturnsAsync(cat);

            IngredientCateRepositoryMock
                .Setup(r => r.DeleteAsync(cat))
                .Returns(Task.CompletedTask);

            CacheServiceMock
                .Setup(c => c.RemoveByPrefixAsync("ingredient-category"))
                .Returns(Task.CompletedTask);

            await Sut.DeleteIngredientCategoryAsync(cat.Id);

            IngredientCateRepositoryMock.VerifyAll();
            CacheServiceMock.VerifyAll();
        }
    }
}
