using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CommentServiceTests
{
    public class CreateAsyncTests : CommentServiceTestBase
    {
        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenRecipeNotFound()
        {
            var recipeId = Guid.NewGuid();
            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync((Recipe?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateAsync(Guid.NewGuid(), recipeId, new CreateCommentRequest()));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenMentionSelf()
        {
            var userId = Guid.NewGuid();
            var recipe = CreateRecipe();
            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            UserRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(true);

            var req = new CreateCommentRequest
            {
                Content = "hi",
                MentionedUserIds = new List<Guid> { userId }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateAsync(userId, recipe.Id, req));
        }

        [Fact]
        public async Task CreateAsync_ShouldCreate_WhenValid()
        {
            var userId = Guid.NewGuid();
            var recipe = CreateRecipe();

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            UserRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(true);
            CommentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Comment>()))
                .ReturnsAsync((Comment c) => c);
            CommentRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(CreateComment());

            await Sut.CreateAsync(userId, recipe.Id, new CreateCommentRequest { Content = "test" });

            CommentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
            NotifierMock.Verify(n => n.SendCommentAddedAsync(recipe.Id, It.IsAny<CommentResponse>()), Times.Once);
        }
    }
}
