using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RatingServiceTests
{
    public class DeleteRatingAsyncTests : RatingServiceTestBase
    {
        [Fact]
        public async Task ShouldThrow_WhenRatingNotFound()
        {
            RatingRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((Rating)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteRatingAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task ShouldThrow_WhenNotOwner()
        {
            var rating = CreateRating(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            RatingRepositoryMock
                .Setup(r => r.GetByIdAsync(rating.Id, null))
                .ReturnsAsync(rating);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteRatingAsync(Guid.NewGuid(), rating.Id));
        }

        [Fact]
        public async Task ShouldDelete_WhenValid()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var rating = CreateRating(Guid.NewGuid(), userId, recipeId);

            RatingRepositoryMock
                .Setup(r => r.GetByIdAsync(rating.Id, null))
                .ReturnsAsync(rating);

            RatingRepositoryMock
                .Setup(r => r.DeleteAsync(rating))
                .Returns(Task.CompletedTask);

            RatingRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Rating, bool>>>(), null))
                .ReturnsAsync(new List<Rating>());

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(CreateRecipe(recipeId));

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);

            NotifierMock
                .Setup(n => n.SendRatingDeletedAsync(recipeId, rating.Id))
                .Returns(Task.CompletedTask);

            await Sut.DeleteRatingAsync(userId, rating.Id);

            RatingRepositoryMock.Verify(r => r.DeleteAsync(rating), Times.Once);
        }
    }
}
