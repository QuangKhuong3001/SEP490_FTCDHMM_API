using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.USDA;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.IngredientServiceTests
{
    public class GetFromUsdaSourceAsyncTests : IngredientServiceTestBase
    {
        [Fact]
        public async Task GetFromUsdaSource_ShouldReturnEmpty_WhenKeywordTooShort()
        {
            var result = await Sut.GetFromUsdaSourceAsync("a");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFromUsdaSource_ShouldReturnDbMapped_WhenDbHasItems()
        {
            var keyword = "apple";
            var ingredients = new List<Ingredient>
            {
                CreateIngredient(),
                CreateIngredient()
            };

            IngredientRepositoryMock
                .Setup(r => r.GetTop5Async(It.IsAny<string>(), default))
                .ReturnsAsync(ingredients);

            var mapped = ingredients.Select(i => new IngredientNameResponse
            {
                Id = i.Id,
                Name = i.Name
            }).ToList();

            MapperMock
                .Setup(m => m.Map<List<IngredientNameResponse>>(ingredients))
                .Returns(mapped);

            var result = await Sut.GetFromUsdaSourceAsync(keyword);

            Assert.Equal(mapped.Count, result.Count());
            IngredientRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetFromUsdaSource_ShouldCreateFromUsda_WhenDbEmptyAndSearchSuccess()
        {
            var keyword = "apple";

            IngredientRepositoryMock
                .Setup(r => r.GetTop5Async(It.IsAny<string>(), default))
                .ReturnsAsync(new List<Ingredient>());

            TranslateServiceMock
                .Setup(t => t.TranslateToEnglishAsync(It.IsAny<string>()))
                .ReturnsAsync("apple");

            TranslateServiceMock
                .Setup(t => t.TranslateToVietnameseAsync("apple"))
                .ReturnsAsync("Táo");

            UsdaApiServiceMock
                .Setup(u => u.SearchAsync("apple"))
                .ReturnsAsync(new UsdaSearchResult { FdcId = 1, Description = "Apple" });

            var detail = new UsdaFoodDetail
            {
                FdcId = 1,
                Description = "Apple detail",
                FoodNutrients = new List<UsdaFoodNutrient>()
            };

            UsdaApiServiceMock
                .Setup(u => u.GetDetailAsync(1))
                .ReturnsAsync(detail);

            TranslateServiceMock
                .Setup(t => t.TranslateToVietnameseAsync(detail.Description))
                .ReturnsAsync("Táo chi tiết");

            IngredientRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                .ReturnsAsync(false);

            IngredientCategoryRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<IngredientCategory, bool>>>(),
                    null,
                    null
                ))
                .ReturnsAsync(new IngredientCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "USDA Imported"
                });

            ImageRepositoryMock
                .Setup(r => r.GetDefaultImageAsync())
                .ReturnsAsync(new Image { Id = Guid.NewGuid(), Key = "images/default.png" });

            NutrientRepositoryMock
                .Setup(r => r.GetAllAsync(null, null))
                .ReturnsAsync(new List<Nutrient>());

            IngredientRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ingredient>()))
                .ReturnsAsync((Ingredient ing) => ing);

            var result = await Sut.GetFromUsdaSourceAsync(keyword);

            Assert.Single(result);

            IngredientRepositoryMock.VerifyAll();
            IngredientCategoryRepositoryMock.VerifyAll();
            TranslateServiceMock.VerifyAll();
            UsdaApiServiceMock.VerifyAll();
            ImageRepositoryMock.VerifyAll();
            NutrientRepositoryMock.VerifyAll();
        }
    }
}
