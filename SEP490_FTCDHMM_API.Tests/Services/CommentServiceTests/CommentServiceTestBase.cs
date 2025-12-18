using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.CommentServiceTests
{
    public abstract class CommentServiceTestBase
    {
        protected readonly Mock<ICommentRepository> CommentRepositoryMock;
        protected readonly Mock<IMapper> MapperMock;
        protected readonly Mock<IRealtimeNotifier> NotifierMock;
        protected readonly Mock<IUserRepository> UserRepositoryMock;
        protected readonly Mock<IRecipeRepository> RecipeRepositoryMock;
        protected readonly Mock<INotificationRepository> NotificationRepositoryMock;

        protected readonly CommentService Sut;

        protected CommentServiceTestBase()
        {
            CommentRepositoryMock = new Mock<ICommentRepository>();
            MapperMock = new Mock<IMapper>();
            NotifierMock = new Mock<IRealtimeNotifier>();
            UserRepositoryMock = new Mock<IUserRepository>();
            RecipeRepositoryMock = new Mock<IRecipeRepository>();
            NotificationRepositoryMock = new Mock<INotificationRepository>();

            Sut = new CommentService(
                CommentRepositoryMock.Object,
                MapperMock.Object,
                UserRepositoryMock.Object,
                RecipeRepositoryMock.Object,
                NotificationRepositoryMock.Object,
                NotifierMock.Object,
                new Mock<IS3ImageService>().Object
            );
        }

        protected static Comment CreateComment(Guid? id = null, Guid? userId = null, Guid? recipeId = null)
        {
            return new Comment
            {
                Id = id ?? Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                RecipeId = recipeId ?? Guid.NewGuid(),
                Content = "Test comment",
                CreatedAtUtc = DateTime.UtcNow,
                Mentions = new List<CommentMention>(),
                Replies = new List<Comment>()
            };
        }

        protected static Recipe CreateRecipe(Guid? id = null, Guid? authorId = null)
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
                Difficulty = DifficultyValue.Medium
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
