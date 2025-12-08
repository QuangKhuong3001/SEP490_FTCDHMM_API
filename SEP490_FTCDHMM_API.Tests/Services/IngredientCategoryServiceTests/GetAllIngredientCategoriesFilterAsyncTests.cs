using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.IngredientCategoryServiceTests
{
    public class GetAllIngredientCategoriesFilterAsyncTests : IngredientCategoryServiceTestBase
    {
        [Fact]
        public async Task GetAllFilter_ShouldReturnPagedResult()
        {
            var categories = new List<IngredientCategory>
            {
                new IngredientCategory { Id = NewId(), Name = "Fruit" }
            };

            IngredientCateRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<IngredientCategory, bool>>>(),
                    It.IsAny<Func<IQueryable<IngredientCategory>, IOrderedQueryable<IngredientCategory>>>(),
                    null,
                    null,
                    null))
                .ReturnsAsync((categories, 1));

            MapperMock
                .Setup(m => m.Map<List<IngredientCategoryResponse>>(categories))
                .Returns(new List<IngredientCategoryResponse>
                {
                    new IngredientCategoryResponse { Name = "Fruit" }
                });

            var req = new IngredientCategoryFilterRequest
            {
                Keyword = "",
                PaginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            var result = await Sut.GetAllIngredientCategoriesFilterAsync(req);

            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);

            IngredientCateRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
