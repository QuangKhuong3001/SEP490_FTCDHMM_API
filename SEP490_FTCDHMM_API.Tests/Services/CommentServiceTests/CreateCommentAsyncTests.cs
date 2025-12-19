using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CommentServiceTests
{
    public class CreateCommentAsyncTests : CommentServiceTestBase
    {
        private static Recipe PostedRecipe(Guid recipeId, Guid authorId) =>
            new()
            {
                Id = recipeId,
                AuthorId = authorId,
                Status = RecipeStatus.Posted
            };

        private static Comment RootComment(Guid id, Guid userId, Guid recipeId) =>
            new()
            {
                Id = id,
                UserId = userId,
                RecipeId = recipeId
            };

        [Fact]
        public async Task ShouldThrow_WhenRecipeNotFound()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync((Recipe?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateCommentAsync(userId, recipeId, new CreateCommentRequest()));

            RecipeRepositoryMock.Verify(r => r.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()), Times.Once);
        }

        [Fact]
        public async Task ShouldThrow_WhenRecipeNotPosted()
        {
            var recipe = new Recipe { Status = RecipeStatus.Locked };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateCommentAsync(Guid.NewGuid(), Guid.NewGuid(), new CreateCommentRequest()));
        }

        [Fact]
        public async Task ShouldThrow_WhenParentCommentNotFound()
        {
            var recipeId = Guid.NewGuid();

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(PostedRecipe(recipeId, Guid.NewGuid()));

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync((Comment?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateCommentAsync(Guid.NewGuid(), recipeId,
                    new CreateCommentRequest { ParentCommentId = Guid.NewGuid() }));
        }

        [Fact]
        public async Task ShouldThrow_WhenParentBelongsToAnotherRecipe()
        {
            var recipeId = Guid.NewGuid();
            var parent = new Comment { RecipeId = Guid.NewGuid() };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(PostedRecipe(recipeId, Guid.NewGuid()));

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(parent);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateCommentAsync(Guid.NewGuid(), recipeId,
                    new CreateCommentRequest { ParentCommentId = Guid.NewGuid() }));
        }

        [Fact]
        public async Task ShouldThrow_WhenMentionUserNotExist()
        {
            var recipeId = Guid.NewGuid();
            var mentionedId = Guid.NewGuid();

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(PostedRecipe(recipeId, Guid.NewGuid()));

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateCommentAsync(Guid.NewGuid(), recipeId,
                    new CreateCommentRequest { MentionedUserIds = [mentionedId] }));
        }

        [Fact]
        public async Task ShouldThrow_WhenUserMentionsSelf()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(PostedRecipe(recipeId, Guid.NewGuid()));

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateCommentAsync(userId, recipeId,
                    new CreateCommentRequest { MentionedUserIds = [userId] }));
        }

        [Fact]
        public async Task ShouldCreateRootComment_AndSendNotifications()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(PostedRecipe(recipeId, authorId));

            CommentRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Comment>()))
                .Callback<Comment>(c => c.Id = commentId)
                .ReturnsAsync((Comment c) => c);

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    commentId,
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(RootComment(commentId, userId, recipeId));

            MapperMock
                .Setup(m => m.Map<CommentResponse>(It.IsAny<Comment>()))
                .Returns(new CommentResponse());

            NotificationCommandServiceMock
                .Setup(r => r.CreateAndSendNotificationAsync(
                    userId, authorId, NotificationType.Comment, recipeId))
                .Returns(Task.CompletedTask);

            NotifierMock
                .Setup(n => n.SendCommentAddedAsync(recipeId, It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            await Sut.CreateCommentAsync(userId, recipeId,
                new CreateCommentRequest { Content = "hello" });

            CommentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
            NotifierMock.Verify(r => r.SendCommentAddedAsync(recipeId, It.IsAny<object>()), Times.Once);
            NotificationCommandServiceMock.VerifyAll();
        }
    }
}
