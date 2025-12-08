using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.IngredientCategoryServiceTests
{
    public class GetIngredientCategoriesAsyncTests : IngredientCategoryServiceTestBase
    {
        [Fact]
        public async Task GetDropBox_ShouldReturnMappedItems()
        {
            var list = new List<IngredientCategory>
            {
                new IngredientCategory { Id = NewId(), Name = "A" },
                new IngredientCategory { Id = NewId(), Name = "B" }
            };

            IngredientCateRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<IngredientCategory, bool>>>(),
                    null))
                .ReturnsAsync(list);

            MapperMock
                .Setup(m => m.Map<IEnumerable<IngredientCategoryResponse>>(It.IsAny<IEnumerable<IngredientCategory>>()))
                .Returns(new List<IngredientCategoryResponse>
                {
                    new IngredientCategoryResponse { Name = "A" },
                    new IngredientCategoryResponse { Name = "B" }
                });

            var req = new IngredientCategorySearchDropboxRequest { Keyword = "" };
            var result = await Sut.GetIngredientCategoriesAsync(req);

            Assert.Equal(2, result.Count());
            IngredientCateRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
