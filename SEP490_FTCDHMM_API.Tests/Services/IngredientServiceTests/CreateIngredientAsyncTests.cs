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
    public class CreateIngredientAsyncTests : IngredientServiceTestBase
    {
        private static IFormFile CreateImage()
        {
            var bytes = new byte[] { 1, 2, 3 };
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "image", "image.jpg");
        }

        private CreateIngredientRequest CreateValidRequest(List<Guid>? categoryIds = null, List<Guid>? nutrientIds = null)
        {
            var catIds = categoryIds ?? new List<Guid> { NewId() };
            var nutIds = nutrientIds ?? new List<Guid> { NewId() };

            return new CreateIngredientRequest
            {
                Name = "Táo đỏ",
                Description = "desc",
                IngredientCategoryIds = catIds,
                Image = CreateImage(),
                Nutrients = nutIds.Select(id => new NutrientRequest
                {
                    NutrientId = id,
                    Value = 10
                }).ToList()
            };
        }

        [Fact]
        public async Task CreateIngredient_ShouldThrowExists_WhenNameAlreadyExists()
        {
            var dto = CreateValidRequest();

            IngredientRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.CreateIngredientAsync(dto));

            Assert.Equal(AppResponseCode.EXISTS, ex.ResponseCode);
            IngredientRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task CreateIngredient_ShouldThrowNotFound_WhenCategoryIdsInvalid()
        {
            var dto = CreateValidRequest();

            IngredientRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                .ReturnsAsync(false);

            IngredientCategoryRepositoryMock
                .Setup(r => r.IdsExistAsync(dto.IngredientCategoryIds))
                .ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.CreateIngredientAsync(dto));

            Assert.Equal(AppResponseCode.NOT_FOUND, ex.ResponseCode);

            IngredientRepositoryMock.VerifyAll();
            IngredientCategoryRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task CreateIngredient_ShouldThrowMissingRequired_WhenMacroNutrientsMissing()
        {
            var dto = CreateValidRequest();

            IngredientRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                .ReturnsAsync(false);

            IngredientCategoryRepositoryMock
                .Setup(r => r.IdsExistAsync(dto.IngredientCategoryIds))
                .ReturnsAsync(true);

            var nutrientIds = dto.Nutrients.Select(n => n.NutrientId).ToList();

            NutrientRepositoryMock
                .Setup(r => r.IdsExistAsync(nutrientIds))
                .ReturnsAsync(true);

            var macroNutrient = new Nutrient { Id = NewId(), Name = "Protein", VietnameseName = "Protein", IsMacroNutrient = true };
            NutrientRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Nutrient, bool>>>(), null))
                .ReturnsAsync(new List<Nutrient> { macroNutrient });

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.CreateIngredientAsync(dto));

            Assert.Equal(AppResponseCode.MISSING_REQUIRED_NUTRIENTS, ex.ResponseCode);

            IngredientRepositoryMock.VerifyAll();
            IngredientCategoryRepositoryMock.VerifyAll();
            NutrientRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task CreateIngredient_ShouldCreateSuccessfully_WhenRequestValid()
        {
            var dto = CreateValidRequest();
            var nutrientIds = dto.Nutrients.Select(n => n.NutrientId).ToList();

            IngredientRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                .ReturnsAsync(false);

            IngredientCategoryRepositoryMock
                .Setup(r => r.IdsExistAsync(dto.IngredientCategoryIds))
                .ReturnsAsync(true);

            NutrientRepositoryMock
                .Setup(r => r.IdsExistAsync(nutrientIds))
                .ReturnsAsync(true);

            var macroNutrients = nutrientIds.Select(id => new Nutrient
            {
                Id = id,
                Name = "Macro",
                VietnameseName = "Macro",
                IsMacroNutrient = true
            }).ToList();

            NutrientRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Nutrient, bool>>>(), null))
                .ReturnsAsync(macroNutrients);

            var categories = dto.IngredientCategoryIds.Select(id => new IngredientCategory
            {
                Id = id,
                Name = "Cate"
            }).ToList();

            IngredientCategoryRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<IngredientCategory, bool>>>(), null))
                .ReturnsAsync(categories);

            S3ImageServiceMock
                .Setup(s => s.UploadImageAsync(dto.Image!, It.IsAny<StorageFolder>()))
                .ReturnsAsync(new Image { Id = Guid.NewGuid(), Key = "images/ingredient.png" });

            IngredientNutritionCalculatorMock
                .Setup(c => c.CalculateCalories(It.IsAny<IEnumerable<NutrientValueInput>>()))
                .Returns(123);

            IngredientRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ingredient>()))
                .ReturnsAsync((Ingredient ing) => ing);

            await Sut.CreateIngredientAsync(dto);

            IngredientRepositoryMock.VerifyAll();
            IngredientCategoryRepositoryMock.VerifyAll();
            NutrientRepositoryMock.VerifyAll();
            S3ImageServiceMock.VerifyAll();
            IngredientNutritionCalculatorMock.VerifyAll();
        }
    }
}
