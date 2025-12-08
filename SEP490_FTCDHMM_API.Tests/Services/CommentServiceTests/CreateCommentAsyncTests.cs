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
        private Recipe CreateRecipe(Guid id, Guid authorId, bool posted)
        {
            var recipe = new Recipe
            {
                Id = id,
                AuthorId = authorId,
                Name = "Recipe name",
                Ration = 1
            };

            recipe.Status = posted ? RecipeStatus.Posted : RecipeStatus.Locked;

            return recipe;
        }

        private AppUser CreateUser(Guid id)
        {
            return new AppUser
            {
                Id = id,
                FirstName = "User",
                LastName = "Test",
                UserName = "user.test",
                Email = "user@test.com",
                Gender = Gender.Male,
                DateOfBirth = DateTime.UtcNow.AddYears(-20),
                ActivityLevel = ActivityLevel.From("SEDENTARY")
            };
        }

        private Comment CreateComment(Guid id, Guid userId, Guid recipeId)
        {
            return new Comment
            {
                Id = id,
                UserId = userId,
                RecipeId = recipeId,
                Content = "content",
                CreatedAtUtc = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenRecipeNotFound()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var request = new CreateCommentRequest();

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync((Recipe)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateCommentAsync(userId, recipeId, request));

            RecipeRepositoryMock.Verify(r => r.GetByIdAsync(recipeId, null), Times.Once);
            CommentRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()), Times.Never);
            CommentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
            NotificationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentAddedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
            NotifierMock.Verify(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenRecipeNotPosted()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var request = new CreateCommentRequest();

            var recipe = CreateRecipe(recipeId, authorId, posted: false);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateCommentAsync(userId, recipeId, request));

            RecipeRepositoryMock.Verify(r => r.GetByIdAsync(recipeId, null), Times.Once);
            CommentRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()), Times.Never);
            CommentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
            NotificationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentAddedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
            NotifierMock.Verify(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenParentCommentNotFound()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var parentId = Guid.NewGuid();

            var request = new CreateCommentRequest
            {
                ParentCommentId = parentId
            };

            var recipe = CreateRecipe(recipeId, authorId, posted: true);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(parentId, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync((Comment)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateCommentAsync(userId, recipeId, request));

            RecipeRepositoryMock.Verify(r => r.GetByIdAsync(recipeId, null), Times.Once);
            CommentRepositoryMock.Verify(r => r.GetByIdAsync(parentId, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()), Times.Once);
            CommentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
            NotificationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentAddedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
            NotifierMock.Verify(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenParentCommentBelongsToAnotherRecipe()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var otherRecipeId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var parentId = Guid.NewGuid();
            var rootParentId = Guid.NewGuid();

            var request = new CreateCommentRequest
            {
                ParentCommentId = parentId
            };

            var recipe = CreateRecipe(recipeId, authorId, posted: true);

            var parent = new Comment
            {
                Id = parentId,
                RecipeId = otherRecipeId,
                ParentCommentId = rootParentId,
                ParentComment = new Comment
                {
                    Id = rootParentId,
                    RecipeId = otherRecipeId
                }
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(parentId, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(parent);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateCommentAsync(userId, recipeId, request));

            RecipeRepositoryMock.Verify(r => r.GetByIdAsync(recipeId, null), Times.Once);
            CommentRepositoryMock.Verify(r => r.GetByIdAsync(parentId, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()), Times.Once);
            CommentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
            NotificationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentAddedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
            NotifierMock.Verify(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenMentionedUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var mentionedId = Guid.NewGuid();

            var request = new CreateCommentRequest
            {
                MentionedUserIds = new List<Guid> { mentionedId }
            };

            var recipe = CreateRecipe(recipeId, authorId, posted: true);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateCommentAsync(userId, recipeId, request));

            CommentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
            NotificationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentAddedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
            NotifierMock.Verify(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenUserMentionsThemself()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var authorId = Guid.NewGuid();

            var request = new CreateCommentRequest
            {
                MentionedUserIds = new List<Guid> { userId }
            };

            var recipe = CreateRecipe(recipeId, authorId, posted: true);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateCommentAsync(userId, recipeId, request));

            CommentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
            NotificationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentAddedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
            NotifierMock.Verify(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateRootComment_WhenValid()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            var recipe = CreateRecipe(recipeId, authorId, posted: true);

            var request = new CreateCommentRequest
            {
                Content = "Hello"
            };

            var savedComment = CreateComment(commentId, userId, recipeId);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            CommentRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Comment>()))
                .Callback<Comment>(c => c.Id = commentId)
                .ReturnsAsync((Comment c) => c);

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(commentId, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(savedComment);

            MapperMock
                .Setup(m => m.Map<CommentResponse>(savedComment))
                .Returns(new CommentResponse());

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                ReceiverId = authorId,
                Type = NotificationType.Comment,
                TargetId = commentId,
                CreatedAtUtc = DateTime.UtcNow
            };

            NotificationRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Notification>()))
                .ReturnsAsync(notification);

            var sender = CreateUser(userId);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(sender);

            NotifierMock
                .Setup(n => n.SendCommentAddedAsync(recipeId, It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            NotifierMock
                .Setup(n => n.SendNotificationAsync(authorId, It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            await Sut.CreateCommentAsync(userId, recipeId, request);

            RecipeRepositoryMock.Verify(r => r.GetByIdAsync(recipeId, null), Times.Once);
            CommentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
            CommentRepositoryMock.Verify(r => r.GetByIdAsync(commentId, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()), Times.Once);
            MapperMock.Verify(m => m.Map<CommentResponse>(savedComment), Times.Once);
            NotifierMock.Verify(n => n.SendCommentAddedAsync(recipeId, It.IsAny<object>()), Times.Once);
            NotificationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Once);
            UserRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()), Times.Once);
            NotifierMock.Verify(n => n.SendNotificationAsync(authorId, It.IsAny<object>()), Times.Once);
        }
    }
}
