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
        public async Task Delete_ShouldThrow_WhenCategoryIsDeleted()
        {
            var cat = new IngredientCategory { Name = "Test", IsDeleted = true };

            IngredientCateRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<IngredientCategory>, IQueryable<IngredientCategory>>>()))
                .ReturnsAsync(cat);

            await Assert.ThrowsAsync<AppException>(() => Sut.DeleteIngredientCategoryAsync(NewId()));
            IngredientCateRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Delete_ShouldSoftDelete_WhenCategoryHasIngredients()
        {
            var cat = new IngredientCategory
            {
                Name = "Category 1",
                Id = NewId(),
                IsDeleted = false,
                Ingredients = new List<Ingredient>
                { new Ingredient
                    {
                        Id = NewId(),
                        Name = "Test",
                        Image = new Image
                        {
                            Id = NewId(),
                            Key = "x",
                            ContentType = "image/png",
                            CreatedAtUTC = DateTime.UtcNow
                        }
                    }
                }
            };

            IngredientCateRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<IngredientCategory>, IQueryable<IngredientCategory>>>()))
                .ReturnsAsync(cat);

            IngredientCateRepositoryMock
                .Setup(r => r.UpdateAsync(It.Is<IngredientCategory>(c => c.IsDeleted)))
                .Returns(Task.CompletedTask);

            await Sut.DeleteIngredientCategoryAsync(cat.Id);

            IngredientCateRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Delete_ShouldHardDelete_WhenNoIngredients()
        {
            var cat = new IngredientCategory
            {
                Name = "Category 1",
                Id = NewId(),
                IsDeleted = false,
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

            await Sut.DeleteIngredientCategoryAsync(cat.Id);

            IngredientCateRepositoryMock.VerifyAll();
        }
    }
}
