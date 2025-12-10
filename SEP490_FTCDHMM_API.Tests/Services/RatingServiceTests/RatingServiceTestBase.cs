using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.RatingServiceTests
{
    public abstract class RatingServiceTestBase
    {
        protected Mock<IRatingRepository> RatingRepositoryMock { get; }
        protected Mock<IRecipeRepository> RecipeRepositoryMock { get; }
        protected Mock<IRealtimeNotifier> NotifierMock { get; }
        protected Mock<IMapper> MapperMock { get; }
        protected RatingService Sut { get; }

        protected RatingServiceTestBase()
        {
            RatingRepositoryMock = new Mock<IRatingRepository>(MockBehavior.Strict);
            RecipeRepositoryMock = new Mock<IRecipeRepository>(MockBehavior.Strict);
            NotifierMock = new Mock<IRealtimeNotifier>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);

            Sut = new RatingService(
                RatingRepositoryMock.Object,
                NotifierMock.Object,
                RecipeRepositoryMock.Object,
                MapperMock.Object);
        }

        protected Rating CreateRating(Guid id, Guid userId, Guid recipeId, int score = 5, string feedback = "good")
        {
            return new Rating
            {
                Id = id,
                UserId = userId,
                RecipeId = recipeId,
                Feedback = feedback,
                Score = score,
                CreatedAtUtc = DateTime.UtcNow
            };
        }

        protected Recipe CreateRecipe(Guid id)
        {
            return new Recipe
            {
                Id = id,
                Status = RecipeStatus.Posted
            };
        }
    }
}
