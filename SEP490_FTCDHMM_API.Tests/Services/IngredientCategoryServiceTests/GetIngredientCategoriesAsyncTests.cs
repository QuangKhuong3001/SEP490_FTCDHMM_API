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
            CacheServiceMock
                .Setup(c => c.GetAsync<IEnumerable<IngredientCategoryResponse>>(
                    It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<IngredientCategoryResponse>)null!);

            IngredientCateRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<IngredientCategory, bool>>>(),
                    It.IsAny<Func<IQueryable<IngredientCategory>, IQueryable<IngredientCategory>>>()))
                .ReturnsAsync(new List<IngredientCategory>
                {
                    new IngredientCategory { Name = "A" }
                });

            MapperMock
                .Setup(m => m.Map<IEnumerable<IngredientCategoryResponse>>(
                    It.IsAny<IEnumerable<IngredientCategory>>()))
                .Returns(new List<IngredientCategoryResponse>
                {
            new IngredientCategoryResponse { Name = "A" }
                });

            CacheServiceMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<IngredientCategoryResponse>>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            var req = new IngredientCategorySearchDropboxRequest
            {
                Keyword = ""
            };

            var result = await Sut.GetIngredientCategoriesAsync(req);

            Assert.Single(result);

            CacheServiceMock.VerifyAll();
            IngredientCateRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
