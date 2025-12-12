using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeQueryServiceTests
{
    public class GetRecipeRatingDetailsAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task GetDetails_ShouldThrow_WhenRecipeNotFound()
        {
            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync((Recipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeRatingDetailsAsync(null, NewId(), new RecipePaginationParams()));

            RecipeRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetDetails_ShouldThrow_WhenUnauthorized()
        {
            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Pending,
                AuthorId = NewId(),
                Ratings = new List<Rating>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeRatingDetailsAsync(null, recipe.Id, new RecipePaginationParams()));

            RecipeRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetDetails_ShouldReturnPagedRatings()
        {
            var recipeId = NewId();
            var userId = NewId();

            var r1 = new Rating
            {
                Id = NewId(),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow,
                User = new AppUser { Id = userId }
            };

            var r2 = new Rating
            {
                Id = NewId(),
                UserId = NewId(),
                CreatedAtUtc = DateTime.UtcNow.AddMinutes(-1),
                User = new AppUser { Id = NewId() }
            };

            var recipe = new Recipe
            {
                Id = recipeId,
                Status = RecipeStatus.Posted,
                AuthorId = NewId(),
                Ratings = new List<Rating> { r1, r2 }
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipeId,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            MapperMock
                .Setup(m => m.Map<IEnumerable<RatingDetailsResponse>>(It.IsAny<List<Rating>>()))
                .Returns((List<Rating> src) =>
                    src.Select(x => new RatingDetailsResponse
                    {
                        Id = x.Id,
                        CreatedAtUtc = x.CreatedAtUtc,
                        UserInteractionResponse = new() { Id = x.UserId }
                    }).ToList()
                );

            var request = new RecipePaginationParams
            {
                PageNumber = 1,
            };

            var result = await Sut.GetRecipeRatingDetailsAsync(userId, recipeId, request);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.True(result.Items.First().IsOwner);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetDetails_ShouldMarkOwnerCorrectly()
        {
            var userId = NewId();
            var recipeId = NewId();

            var r1 = new Rating
            {
                Id = NewId(),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow,
                User = new AppUser { Id = userId }
            };

            var r2 = new Rating
            {
                Id = NewId(),
                UserId = NewId(),
                CreatedAtUtc = DateTime.UtcNow.AddHours(-1),
                User = new AppUser { Id = NewId() }
            };

            var recipe = new Recipe
            {
                Id = recipeId,
                Status = RecipeStatus.Posted,
                AuthorId = NewId(),
                Ratings = new List<Rating> { r1, r2 }
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipeId,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            MapperMock
                .Setup(m => m.Map<IEnumerable<RatingDetailsResponse>>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    var list = (IEnumerable<Rating>)src;
                    return list.Select(x => new RatingDetailsResponse
                    {
                        Id = x.Id,
                        CreatedAtUtc = x.CreatedAtUtc,
                        UserInteractionResponse = new() { Id = x.UserId }
                    }).ToList();
                });

            var req = new RecipePaginationParams
            {
                PageNumber = 1,
            };

            var result = await Sut.GetRecipeRatingDetailsAsync(userId, recipeId, req);

            var items = result.Items.ToList();

            Assert.Contains(items, x =>
                x.UserInteractionResponse.Id == userId && x.IsOwner);

            Assert.DoesNotContain(items, x =>
                x.UserInteractionResponse.Id != userId && x.IsOwner);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
