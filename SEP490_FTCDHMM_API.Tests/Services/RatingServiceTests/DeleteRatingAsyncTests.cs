using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RatingServiceTests
{
    public class DeleteRatingAsyncTests : RatingServiceTestBase
    {
        [Fact]
        public async Task DeleteRatingAsync_ShouldThrow_WhenNotOwner()
        {
            var ratingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            RatingRepositoryMock
                .Setup(r => r.GetByIdAsync(ratingId, null))
                .ReturnsAsync(CreateRating(ratingId, Guid.NewGuid(), Guid.NewGuid()));

            await Assert.ThrowsAsync<AppException>(() => Sut.DeleteRatingAsync(userId, ratingId));
        }

        [Fact]
        public async Task DeleteRatingAsync_ShouldDeleteAndNotify()
        {
            var ratingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            var rating = CreateRating(ratingId, userId, recipeId);

            RatingRepositoryMock
                .Setup(r => r.GetByIdAsync(ratingId, null))
                .ReturnsAsync(rating);
            RatingRepositoryMock
                .Setup(r => r.DeleteAsync(rating))
                .Returns(Task.CompletedTask);
            RatingRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Rating, bool>>>(),
                    It.IsAny<Func<IQueryable<Rating>, IQueryable<Rating>>>()))
                .ReturnsAsync(new List<Rating>());
            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(CreateRecipe(recipeId));
            RecipeRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);
            NotifierMock.Setup(n => n.SendRatingDeletedAsync(recipeId, ratingId))
                .Returns(Task.CompletedTask);

            await Sut.DeleteRatingAsync(userId, ratingId);

            RatingRepositoryMock.Verify(r => r.DeleteAsync(rating), Times.Once);
            NotifierMock.Verify(n => n.SendRatingDeletedAsync(recipeId, ratingId), Times.Once);
        }
    }
}
