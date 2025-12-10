using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RatingServiceTests
{
    public class AddOrUpdateRatingAsyncTests : RatingServiceTestBase
    {
        [Fact]
        public async Task AddOrUpdateRatingAsync_ShouldThrow_WhenFeedbackRequired()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var request = new RatingRequest { Score = 2, Feedback = null };

            await Assert.ThrowsAsync<AppException>(() => Sut.AddOrUpdateRatingAsync(userId, recipeId, request));
        }

        [Fact]
        public async Task AddOrUpdateRatingAsync_ShouldAddNewRating_WhenNotExists()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var request = new RatingRequest { Score = 5, Feedback = "ok" };

            var recipe = CreateRecipe(recipeId);
            var includeRating = (Func<IQueryable<Rating>, IQueryable<Rating>>?)null;
            var includeRecipe = (Func<IQueryable<Recipe>, IQueryable<Recipe>>?)null;

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, includeRecipe))
                .ReturnsAsync(recipe);

            RatingRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Rating, DateTime>>>(),
                    It.IsAny<Expression<Func<Rating, bool>>>(),
                    includeRating))
                .ReturnsAsync((Rating)null!);

            RatingRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Rating>()))
                .ReturnsAsync((Rating r) => r);

            RatingRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Rating, bool>>>(),
                    includeRating))
                .ReturnsAsync(new List<Rating> { new Rating { Score = 5 } });

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);

            MapperMock
                .Setup(m => m.Map<RatingDetailsResponse>(It.IsAny<Rating>()))
                .Returns(new RatingDetailsResponse());

            NotifierMock
                .Setup(n => n.SendRatingUpdateAsync(recipeId, It.IsAny<RatingDetailsResponse>()))
                .Returns(Task.CompletedTask);

            await Sut.AddOrUpdateRatingAsync(userId, recipeId, request);

            RatingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Rating>()), Times.Once);
            NotifierMock.Verify(n => n.SendRatingUpdateAsync(recipeId, It.IsAny<RatingDetailsResponse>()), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateRatingAsync_ShouldUpdateExistingRating()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            var existing = CreateRating(Guid.NewGuid(), userId, recipeId);
            var recipe = CreateRecipe(recipeId);

            var request = new RatingRequest { Score = 4, Feedback = "nice" };

            var includeRating = (Func<IQueryable<Rating>, IQueryable<Rating>>?)null;
            var includeRecipe = (Func<IQueryable<Recipe>, IQueryable<Recipe>>?)null;

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, includeRecipe))
                .ReturnsAsync(recipe);

            RatingRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Rating, DateTime>>>(),
                    It.IsAny<Expression<Func<Rating, bool>>>(),
                    includeRating))
                .ReturnsAsync(existing);

            RatingRepositoryMock
                .Setup(r => r.UpdateAsync(existing))
                .Returns(Task.CompletedTask);

            RatingRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Rating, bool>>>(),
                    includeRating))
                .ReturnsAsync(new List<Rating> { existing });

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);

            MapperMock
                .Setup(m => m.Map<RatingDetailsResponse>(existing))
                .Returns(new RatingDetailsResponse());

            NotifierMock
                .Setup(n => n.SendRatingUpdateAsync(recipeId, It.IsAny<RatingDetailsResponse>()))
                .Returns(Task.CompletedTask);

            await Sut.AddOrUpdateRatingAsync(userId, recipeId, request);

            RatingRepositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
            NotifierMock.Verify(n => n.SendRatingUpdateAsync(recipeId, It.IsAny<RatingDetailsResponse>()), Times.Once);
        }
    }
}
