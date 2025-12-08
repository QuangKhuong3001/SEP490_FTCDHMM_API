using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.IngredientCategoryServiceTests
{
    public class CreateIngredientCategoryAsyncTests : IngredientCategoryServiceTestBase
    {
        [Fact]
        public async Task Create_ShouldThrow_WhenNameExists()
        {
            IngredientCateRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<IngredientCategory, bool>>>()))
                .ReturnsAsync(true);

            var req = new CreateIngredientCategoryRequest { Name = "Vegetable" };

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateIngredientCategoryAsync(req));

            IngredientCateRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Create_ShouldSucceed()
        {
            IngredientCateRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<IngredientCategory, bool>>>()))
                .ReturnsAsync(false);

            IngredientCateRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<IngredientCategory>()))
                .ReturnsAsync(new IngredientCategory { Name = "New" });

            var req = new CreateIngredientCategoryRequest { Name = "Fruit" };

            await Sut.CreateIngredientCategoryAsync(req);

            IngredientCateRepositoryMock.VerifyAll();
        }
    }
}
