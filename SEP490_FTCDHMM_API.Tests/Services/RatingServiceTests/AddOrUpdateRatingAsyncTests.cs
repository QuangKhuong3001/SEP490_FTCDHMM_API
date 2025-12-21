using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RatingServiceTests
{
    public class AddOrUpdateRatingAsyncTests : RatingServiceTestBase
    {
        [Fact]
        public async Task ShouldThrow_WhenLowScoreWithoutFeedback()
        {
            var request = new RatingRequest { Score = 3, Feedback = null };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.AddOrUpdateRatingAsync(Guid.NewGuid(), Guid.NewGuid(), request));
        }

        [Fact]
        public async Task ShouldThrow_WhenRecipeNotFound()
        {
            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((Recipe)null!);

            var request = new RatingRequest { Score = 5, Feedback = "ok" };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.AddOrUpdateRatingAsync(Guid.NewGuid(), Guid.NewGuid(), request));
        }

        [Fact]
        public async Task ShouldAddNewRating_WhenNotExists()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipe = CreateRecipe(recipeId);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            RatingRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Rating, DateTime>>>(),
                    It.IsAny<Expression<Func<Rating, bool>>>(),
                    null))
                .ReturnsAsync((Rating)null!);

            RatingRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Rating>()))
                .ReturnsAsync((Rating r) => r);

            RatingRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Rating, bool>>>(), null))
                .ReturnsAsync(new List<Rating>
                {
                    new Rating { Score = 5 }
                });

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);

            MapperMock
                .Setup(m => m.Map<RatingDetailsResponse>(It.IsAny<Rating>()))
                .Returns(new RatingDetailsResponse());

            NotifierMock
                .Setup(n => n.SendRatingUpdateAsync(recipeId, It.IsAny<RatingDetailsResponse>()))
                .Returns(Task.CompletedTask);

            NotificationCommandServiceMock
                .Setup(n => n.CreateAndSendNotificationAsync(
                    userId,
                    recipe.AuthorId,
                    NotificationType.Rating,
                    recipeId))
                .Returns(Task.CompletedTask);

            await Sut.AddOrUpdateRatingAsync(userId, recipeId, new RatingRequest
            {
                Score = 5,
                Feedback = "ok"
            });

            RatingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Rating>()), Times.Once);
        }

        [Fact]
        public async Task ShouldUpdateExistingRating()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipe = CreateRecipe(recipeId);
            var rating = CreateRating(Guid.NewGuid(), userId, recipeId);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            RatingRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Rating, DateTime>>>(),
                    It.IsAny<Expression<Func<Rating, bool>>>(),
                    null))
                .ReturnsAsync(rating);

            RatingRepositoryMock
                .Setup(r => r.UpdateAsync(rating))
                .Returns(Task.CompletedTask);

            RatingRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Rating, bool>>>(), null))
                .ReturnsAsync(new List<Rating> { rating });

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);

            MapperMock
                .Setup(m => m.Map<RatingDetailsResponse>(rating))
                .Returns(new RatingDetailsResponse());

            NotifierMock
                .Setup(n => n.SendRatingUpdateAsync(recipeId, It.IsAny<RatingDetailsResponse>()))
                .Returns(Task.CompletedTask);

            NotificationCommandServiceMock
                .Setup(n => n.CreateAndSendNotificationAsync(
                    userId,
                    recipe.AuthorId,
                    NotificationType.Rating,
                    recipeId))
                .Returns(Task.CompletedTask);

            await Sut.AddOrUpdateRatingAsync(userId, recipeId, new RatingRequest
            {
                Score = 4,
                Feedback = "nice"
            });

            RatingRepositoryMock.Verify(r => r.UpdateAsync(rating), Times.Once);
        }
    }
}
