using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.Nutrient;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.IngredientServiceTests
{
    public class UpdateIngredientAsyncTests : IngredientServiceTestBase
    {
        private static IFormFile CreateImage()
        {
            var bytes = new byte[] { 4, 5, 6 };
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "image", "image.jpg");
        }

        private UpdateIngredientRequest CreateRequest(Guid nutrientId, List<Guid> categoryIds, bool includeImage)
        {
            return new UpdateIngredientRequest
            {
                Description = "updated",
                Image = includeImage ? CreateImage() : null,
                IngredientCategoryIds = categoryIds,
                Nutrients = new List<NutrientRequest>
                {
                    new NutrientRequest { NutrientId = nutrientId, Value = 20 }
                }
            };
        }

        [Fact]
        public async Task UpdateIngredient_ShouldThrowNotFound_WhenIngredientNotExists()
        {
            var id = NewId();
            var nutrientId = NewId();
            var dto = CreateRequest(nutrientId, new List<Guid> { NewId() }, false);

            UnitOfWorkMock
                .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>((op) => op());

            IngredientRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<Ingredient>, IQueryable<Ingredient>>>()))
                .ReturnsAsync((Ingredient?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.UpdateIngredientAsync(id, dto));

            Assert.Equal(AppResponseCode.NOT_FOUND, ex.ResponseCode);

            UnitOfWorkMock.VerifyAll();
            IngredientRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task UpdateIngredient_ShouldUpdateSuccessfully_WhenRequestValid()
        {
            var ingredientId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var proteinId = Guid.NewGuid();
            var carbId = Guid.NewGuid();
            var fatId = Guid.NewGuid();

            var ingredient = new Ingredient
            {
                Id = ingredientId,
                Name = "Old",
                LastUpdatedUtc = now,
                Categories = new List<IngredientCategory>(),
                IngredientNutrients = new List<IngredientNutrient>
        {
            new() { NutrientId = proteinId, Value = 10 },
            new() { NutrientId = carbId, Value = 20 },
            new() { NutrientId = fatId, Value = 5 }
        }
            };

            var dto = new UpdateIngredientRequest
            {
                LastUpdatedUtc = now,
                Description = "New desc",
                IngredientCategoryIds = new List<Guid>(),
                Nutrients = new List<NutrientRequest>
                {
                    new() { NutrientId = proteinId, Value = 15 },
                    new() { NutrientId = carbId, Value = 25 },
                    new() { NutrientId = fatId, Value = 8 }
                }
            };

            IngredientRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    ingredientId,
                    It.IsAny<Func<IQueryable<Ingredient>, IQueryable<Ingredient>>>()))
                .ReturnsAsync(ingredient);

            NutrientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            NutrientRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Nutrient, bool>>>(),
                    It.IsAny<Func<IQueryable<Nutrient>, IQueryable<Nutrient>>>()))
                .ReturnsAsync(new List<Nutrient>
                {
                    new() { Id = proteinId, IsMacroNutrient = true },
                    new() { Id = carbId, IsMacroNutrient = true },
                    new() { Id = fatId, IsMacroNutrient = true }
                });

            IngredientCategoryRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            IngredientCategoryRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<IngredientCategory, bool>>>(),
                    It.IsAny<Func<IQueryable<IngredientCategory>, IQueryable<IngredientCategory>>>()))
                .ReturnsAsync(new List<IngredientCategory>());

            IngredientNutritionCalculatorMock
                .Setup(c => c.CalculateCalories(It.IsAny<IEnumerable<NutrientValueInput>>()))
                .Returns(100m);

            IngredientRepositoryMock
                .Setup(r => r.UpdateAsync(ingredient))
                .Returns(Task.CompletedTask);

            CacheServiceMock
                .Setup(c => c.RemoveByPrefixAsync("ingredient"))
                .Returns(Task.CompletedTask);

            UnitOfWorkMock
                .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(f => f());

            await Sut.UpdateIngredientAsync(ingredientId, dto);

            IngredientRepositoryMock.VerifyAll();
            NutrientRepositoryMock.VerifyAll();
            IngredientCategoryRepositoryMock.VerifyAll();
            IngredientNutritionCalculatorMock.VerifyAll();
            CacheServiceMock.VerifyAll();
        }
    }
}
