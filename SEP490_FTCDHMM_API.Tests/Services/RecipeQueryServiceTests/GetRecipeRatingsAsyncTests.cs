using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.RecipeQueryServiceTests
{
    public class GetRecipeRatingsAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task GetRatings_ShouldThrow_WhenRecipeNotFound()
        {
            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((Recipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeRatingsAsync(null, NewId()));

            RecipeRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetRatings_ShouldThrow_WhenUnauthorized_AndUserIsNull()
        {
            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Pending,
                AuthorId = NewId()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipe.Id, null))
                .ReturnsAsync(recipe);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeRatingsAsync(null, recipe.Id));

            RecipeRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetRatings_ShouldAllowOwner_WhenStatusNotPosted()
        {
            var userId = NewId();

            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Pending,
                AuthorId = userId
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipe.Id, null))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new AppUser
                {
                    Id = userId,
                    Role = new AppRole
                    {
                        RolePermissions = new List<AppRolePermission>()
                    }
                });

            MapperMock
                .Setup(m => m.Map<RecipeRatingResponse>(recipe))
                .Returns(new RecipeRatingResponse
                {
                    RatingCount = recipe.RatingCount,
                    AvgRating = recipe.AvgRating
                });

            var result = await Sut.GetRecipeRatingsAsync(userId, recipe.Id);

            Assert.Equal(recipe.RatingCount, result.RatingCount);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
            UserRepositoryMock.VerifyAll();
        }


        [Fact]
        public async Task GetRatings_ShouldAllowUserWithPermission_WhenStatusNotPosted()
        {
            var userId = NewId();

            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Pending,
                AuthorId = NewId()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipe.Id, null))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new AppUser
                {
                    Id = userId,
                    Role = new AppRole
                    {
                        RolePermissions = new List<AppRolePermission>
                        {
                            new AppRolePermission
                            {
                                PermissionAction = new PermissionAction
                                {
                                    PermissionDomain = new PermissionDomain
                                    {
                                        Name = PermissionValue.Recipe_ManagementView.Domain
                                    },
                                    Name = PermissionValue.Recipe_ManagementView.Action
                                }
                            }
                        }
                    }
                });

            MapperMock
                .Setup(m => m.Map<RecipeRatingResponse>(recipe))
                .Returns(new RecipeRatingResponse
                {
                    AvgRating = recipe.AvgRating,
                    RatingCount = recipe.RatingCount
                });

            var result = await Sut.GetRecipeRatingsAsync(userId, recipe.Id);

            Assert.NotNull(result);
            Assert.Equal(recipe.AvgRating, result.AvgRating);

            RecipeRepositoryMock.VerifyAll();
            UserRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetRatings_ShouldReturnMappedResult_WhenPosted()
        {
            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                AvgRating = 4.7,
                Ratings = new List<Rating>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipe.Id, null))
                .ReturnsAsync(recipe);

            MapperMock
                .Setup(m => m.Map<RecipeRatingResponse>(recipe))
                .Returns(new RecipeRatingResponse
                {
                    RatingCount = recipe.RatingCount,
                    AvgRating = recipe.AvgRating
                });

            var result = await Sut.GetRecipeRatingsAsync(null, recipe.Id);

            Assert.NotNull(result);
            Assert.Equal(recipe.AvgRating, result.AvgRating);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
