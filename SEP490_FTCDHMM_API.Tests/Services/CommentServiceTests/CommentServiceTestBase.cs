using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.CommentServiceTests
{
    public abstract class CommentServiceTestBase
    {
        protected Mock<ICommentRepository> CommentRepositoryMock { get; }
        protected Mock<ICommentMentionRepository> CommentMentionRepositoryMock { get; }
        protected Mock<IMapper> MapperMock { get; }
        protected Mock<IRealtimeNotifier> NotifierMock { get; }
        protected Mock<IUserRepository> UserRepositoryMock { get; }
        protected Mock<IRecipeRepository> RecipeRepositoryMock { get; }
        protected Mock<INotificationCommandService> NotificationCommandServiceMock { get; }

        protected CommentService Sut { get; }

        protected CommentServiceTestBase()
        {
            CommentRepositoryMock = new Mock<ICommentRepository>(MockBehavior.Strict);
            CommentMentionRepositoryMock = new Mock<ICommentMentionRepository>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);
            NotifierMock = new Mock<IRealtimeNotifier>(MockBehavior.Strict);
            UserRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
            RecipeRepositoryMock = new Mock<IRecipeRepository>(MockBehavior.Strict);
            NotificationCommandServiceMock = new Mock<INotificationCommandService>(MockBehavior.Strict);

            Sut = new CommentService(
                CommentRepositoryMock.Object,
                MapperMock.Object,
                UserRepositoryMock.Object,
                RecipeRepositoryMock.Object,
                CommentMentionRepositoryMock.Object,
                NotificationCommandServiceMock.Object,
                NotifierMock.Object
            );
        }

        protected static Comment CreateComment(
            Guid? id = null,
            Guid? userId = null,
            Guid? parentId = null,
            Guid? recipeId = null)
        {
            return new Comment
            {
                Id = id ?? Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                RecipeId = recipeId ?? Guid.NewGuid(),
                Content = "Test comment",
                ParentCommentId = parentId,
                ParentComment = parentId != null ? new Comment { Id = parentId.Value } : null,
                CreatedAtUtc = DateTime.UtcNow,
                Mentions = new List<CommentMention>(),
                Replies = new List<Comment>()
            };
        }


        protected static Recipe CreateRecipe(
            Guid? id = null,
            Guid? authorId = null)
        {
            return new Recipe
            {
                Id = id ?? Guid.NewGuid(),
                AuthorId = authorId ?? Guid.NewGuid(),
                Name = "Recipe Test",
                Description = "Desc",
                Ration = 1,
                CookTime = 10,
                CreatedAtUtc = DateTime.UtcNow,
                Difficulty = DifficultyValue.Medium,
                Status = RecipeStatus.Posted
            };
        }

        protected static AppUser CreateUser(Guid? id = null)
        {
            return new AppUser
            {
                Id = id ?? Guid.NewGuid(),
                FirstName = "Nguyen",
                LastName = "Test",
                Email = "test@example.com",
                UserName = "testuser",
                Gender = Gender.Male,
                DateOfBirth = DateTime.UtcNow.AddYears(-20)
            };
        }
    }
}
