using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.RecipeQueryServiceTests
{
    public class GetSavedRecipesAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task GetSaved_ShouldThrow_WhenUserNotFound()
        {
            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((AppUser)null!);

            var req = new SaveRecipeFilterRequest
            {
                PaginationParams = new RecipePaginationParams()
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.GetSavedRecipesAsync(NewId(), req));
            UserRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetSaved_ShouldReturnPagedResult()
        {
            var userId = NewId();

            var user = new AppUser { Id = userId };

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, null))
                .ReturnsAsync(user);

            var recipe1 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                Name = "A",
                Author = new AppUser()
            };

            var recipe2 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                Name = "B",
                Author = new AppUser()
            };

            var saved1 = new UserSaveRecipe
            {
                UserId = userId,
                Recipe = recipe1,
                CreatedAtUtc = DateTime.UtcNow
            };

            var saved2 = new UserSaveRecipe
            {
                UserId = userId,
                Recipe = recipe2,
                CreatedAtUtc = DateTime.UtcNow
            };

            var items = new List<UserSaveRecipe> { saved1, saved2 };

            UserSaveRecipeRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<UserSaveRecipe, bool>>>(),
                    It.IsAny<Func<IQueryable<UserSaveRecipe>, IOrderedQueryable<UserSaveRecipe>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<UserSaveRecipe>, IQueryable<UserSaveRecipe>>?>()
                ))
                .ReturnsAsync((items, items.Count));

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeResponse>
                {
                    new() { Id = recipe1.Id },
                    new() { Id = recipe2.Id }
                });

            var req = new SaveRecipeFilterRequest
            {
                PaginationParams = new RecipePaginationParams
                {
                    PageNumber = 1,
                }
            };

            var result = await Sut.GetSavedRecipesAsync(userId, req);

            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());

            UserRepositoryMock.VerifyAll();
            UserSaveRecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
