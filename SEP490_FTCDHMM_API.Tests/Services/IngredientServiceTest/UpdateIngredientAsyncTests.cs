using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.Nutrient;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
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
                .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((op, ct) => op(ct));

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
            var id = NewId();
            var nutrientId = NewId();
            var categoryId = NewId();

            var ingredient = CreateIngredient(id);
            ingredient.IngredientNutrients.Add(new IngredientNutrient
            {
                IngredientId = id,
                NutrientId = nutrientId,
                Value = 5,
                Ingredient = ingredient,
                Nutrient = new Nutrient
                {
                    Id = nutrientId,
                    Name = "Protein",
                    VietnameseName = "Protein",
                    Unit = new NutrientUnit { Id = NewId(), Name = "g", Symbol = "g" }
                }
            });

            var dto = CreateRequest(nutrientId, new List<Guid> { categoryId }, true);

            UnitOfWorkMock
                .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((op, ct) => op(ct));

            IngredientRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<Ingredient>, IQueryable<Ingredient>>>()))
                .ReturnsAsync(ingredient);

            IngredientCategoryRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            var nutrientIds = dto.Nutrients.Select(n => n.NutrientId).ToList();

            NutrientRepositoryMock
                .Setup(r => r.IdsExistAsync(nutrientIds))
                .ReturnsAsync(true);

            var macros = nutrientIds.Select(x => new Nutrient
            {
                Id = x,
                Name = "Macro",
                VietnameseName = "Macro",
                IsMacroNutrient = true
            }).ToList();

            NutrientRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Nutrient, bool>>>(), null))
                .ReturnsAsync(macros);

            var categories = new List<IngredientCategory>
            {
                new IngredientCategory { Id = categoryId, Name = "Cate" }
            };

            IngredientCategoryRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<IngredientCategory, bool>>>(), null))
                .ReturnsAsync(categories);

            S3ImageServiceMock
                .Setup(s => s.UploadImageAsync(dto.Image!, StorageFolder.INGREDIENTS, null))
                .ReturnsAsync(new Image { Id = Guid.NewGuid(), Key = "images/new.png" });

            IngredientNutritionCalculatorMock
                .Setup(c => c.CalculateCalories(It.IsAny<IEnumerable<NutrientValueInput>>()))
                .Returns(200);

            IngredientRepositoryMock
                .Setup(r => r.UpdateAsync(ingredient))
                .Returns(Task.CompletedTask);

            S3ImageServiceMock
                .Setup(s => s.DeleteImageAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            UnitOfWorkMock
                .Setup(u => u.RegisterAfterCommit(It.IsAny<Func<Task>>()))
                .Callback<Func<Task>>(async f => await f());

            await Sut.UpdateIngredientAsync(id, dto);

            IngredientRepositoryMock.VerifyAll();
            NutrientRepositoryMock.VerifyAll();
            IngredientCategoryRepositoryMock.VerifyAll();
            S3ImageServiceMock.VerifyAll();
            IngredientNutritionCalculatorMock.VerifyAll();
            UnitOfWorkMock.VerifyAll();
        }
    }
}
