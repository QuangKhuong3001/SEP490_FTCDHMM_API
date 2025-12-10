using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.RatingServiceTests
{
    public class DeleteRatingByManagerAsyncTests : RatingServiceTestBase
    {
        [Fact]
        public async Task DeleteRatingByManagerAsync_ShouldDeleteAndNotify()
        {
            var ratingId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var rating = CreateRating(ratingId, Guid.NewGuid(), recipeId);

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
            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);

            NotifierMock.Setup(n => n.SendRatingDeletedAsync(recipeId, ratingId)).Returns(Task.CompletedTask);

            await Sut.DeleteRatingByManagerAsync(ratingId);

            RatingRepositoryMock.Verify(r => r.DeleteAsync(rating), Times.Once);
            NotifierMock.Verify(n => n.SendRatingDeletedAsync(recipeId, ratingId), Times.Once);
        }
    }
}
