using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.RecipeQueryServiceTests
{
    public class GetRecipeDetailsByPermissionAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task DetailsByPermission_ShouldThrow_WhenRecipeNotFound()
        {
            var id = NewId();

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync((Recipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeDetailsByPermissionAsync(id));

            RecipeRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DetailsByPermission_ShouldThrow_WhenRecipeDeleted()
        {
            var id = NewId();

            var recipe = new Recipe
            {
                Id = id,
                Status = RecipeStatus.Deleted
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeDetailsByPermissionAsync(id));

            RecipeRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DetailsByPermission_ShouldReturnMappedResult_WhenValid()
        {
            var id = NewId();

            var recipe = new Recipe
            {
                Id = id,
                Status = RecipeStatus.Posted,
                Author = new AppUser { Id = NewId(), Avatar = new Image() },
                Image = new Image(),
                Labels = new List<Label>(),
                RecipeUserTags = new List<RecipeUserTag>(),
                CookingSteps = new List<CookingStep>(),
                RecipeIngredients = new List<RecipeIngredient>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse { Id = id });

            var result = await Sut.GetRecipeDetailsByPermissionAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task DetailsByPermission_ShouldCallRepositoryWithIncludeDelegate()
        {
            var id = NewId();

            var recipe = new Recipe
            {
                Id = id,
                Status = RecipeStatus.Posted,
                RecipeIngredients = new List<RecipeIngredient>()
            };

            Func<IQueryable<Recipe>, IQueryable<Recipe>> capturedInclude = null!;

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .Callback<Guid, Func<IQueryable<Recipe>, IQueryable<Recipe>>>((_, incl) =>
                {
                    capturedInclude = incl;
                })
                .ReturnsAsync(recipe);

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse { Id = id });

            var res = await Sut.GetRecipeDetailsByPermissionAsync(id);

            Assert.NotNull(capturedInclude);
            Assert.IsType<RecipeDetailsResponse>(res);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
