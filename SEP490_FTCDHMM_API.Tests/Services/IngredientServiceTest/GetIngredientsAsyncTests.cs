using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Tests.Services.IngredientServiceTests
{
    public class GetIngredientsAsyncTests : IngredientServiceTestBase
    {
        [Fact]
        public async Task GetIngredients_ShouldThrowNotFound_WhenCategoryIdsInvalid()
        {
            var dto = new IngredientFilterRequest
            {
                CategoryIds = new List<Guid> { Guid.NewGuid() },
                PaginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            IngredientCategoryRepositoryMock
                .Setup(r => r.IdsExistAsync(dto.CategoryIds))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<AppException>(() => Sut.GetIngredientsAsync(dto));

            IngredientCategoryRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetIngredients_ShouldApplyFilter_WhenCategoryIdsProvided()
        {
            var categoryId = Guid.NewGuid();

            var dto = new IngredientFilterRequest
            {
                CategoryIds = new List<Guid> { categoryId },
                Keyword = "apple",
                PaginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            IngredientCategoryRepositoryMock
                .Setup(r => r.IdsExistAsync(dto.CategoryIds))
                .ReturnsAsync(true);

            var list = new List<Ingredient> { CreateIngredient(), CreateIngredient() };

            IngredientRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    dto.PaginationParams.PageNumber,
                    dto.PaginationParams.PageSize,
                    It.IsAny<Expression<Func<Ingredient, bool>>>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IOrderedQueryable<Ingredient>>>(),
                    dto.Keyword.NormalizeVi(),
                    It.IsAny<string[]>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IQueryable<Ingredient>>>()
                ))
                .ReturnsAsync((list, list.Count));

            var mapped = new List<IngredientResponse>
            {
                new IngredientResponse { Id = list[0].Id, Name = list[0].Name },
                new IngredientResponse { Id = list[1].Id, Name = list[1].Name }
            };

            MapperMock
                .Setup(m => m.Map<List<IngredientResponse>>(list))
                .Returns(mapped);

            var result = await Sut.GetIngredientsAsync(dto);

            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);

            IngredientCategoryRepositoryMock.VerifyAll();
            IngredientRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetIngredients_ShouldNotUseFilter_WhenCategoryIdsEmpty()
        {
            var dto = new IngredientFilterRequest
            {
                CategoryIds = new List<Guid>(),
                Keyword = "apple",
                PaginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            var list = new List<Ingredient> { CreateIngredient() };

            IngredientRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    dto.PaginationParams.PageNumber,
                    dto.PaginationParams.PageSize,
                    null,
                    It.IsAny<Func<IQueryable<Ingredient>, IOrderedQueryable<Ingredient>>>(),
                    dto.Keyword.NormalizeVi(),
                    It.IsAny<string[]>(),
                    It.IsAny<Func<IQueryable<Ingredient>, IQueryable<Ingredient>>>()
                ))
                .ReturnsAsync((list, list.Count));

            var mapped = new List<IngredientResponse>
            {
                new IngredientResponse { Id = list[0].Id, Name = list[0].Name }
            };

            MapperMock
                .Setup(m => m.Map<List<IngredientResponse>>(list))
                .Returns(mapped);

            var result = await Sut.GetIngredientsAsync(dto);

            Assert.Single(result.Items);

            IngredientRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
