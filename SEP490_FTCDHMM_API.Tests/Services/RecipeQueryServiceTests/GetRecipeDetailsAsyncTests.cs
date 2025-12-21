using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeQueryServiceTests
{
    public class GetRecipeDetailsAsyncTests : RecipeQueryServiceTestBase
    {
        private void SetupUser(Guid id, AppUser? user = null)
        {
            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user ?? new AppUser
                {
                    Id = id,
                    Role = new AppRole { RolePermissions = new List<AppRolePermission>() },
                    DietRestrictions = new List<UserDietRestriction>()
                });
        }

        [Fact]
        public async Task Details_ShouldThrow_WhenRecipeNotFound()
        {
            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync((Recipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeDetailsAsync(null, NewId()));
        }

        [Fact]
        public async Task Details_ShouldThrow_WhenStatusNotPosted_AndGuest()
        {
            var recipe = new Recipe
            {
                Id = NewId(),
                AuthorId = NewId(),
                Status = RecipeStatus.Pending
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeDetailsAsync(null, recipe.Id));
        }

        [Fact]
        public async Task Details_ShouldThrow_WhenStatusNotPosted_AndNotOwner()
        {
            var userId = NewId();
            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Pending,
                AuthorId = NewId()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipe.Id, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            SetupUser(userId);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeDetailsAsync(userId, recipe.Id));
        }

        [Fact]
        public async Task Details_Guest_ShouldReturnMappedResult_AndNotCreateView()
        {
            var recipe = new Recipe
            {
                Id = NewId(),
                AuthorId = NewId(),
                Status = RecipeStatus.Posted,
                RecipeIngredients = new List<RecipeIngredient>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()
                ))
                .ReturnsAsync(recipe);

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse { Id = recipe.Id });

            var result = await Sut.GetRecipeDetailsAsync(null, recipe.Id);

            Assert.Equal(recipe.Id, result.Id);

            UserRecipeViewRepositoryMock.VerifyNoOtherCalls();
            UserSaveRecipeRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Details_NotOwner_ShouldCreateView_AndUpdateViewCount()
        {
            var viewerId = NewId();
            var recipe = new Recipe
            {
                Id = NewId(),
                AuthorId = NewId(),
                Status = RecipeStatus.Posted,
                RecipeIngredients = new List<RecipeIngredient>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipe.Id, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            SetupUser(viewerId);

            UserRecipeViewRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RecipeUserView, bool>>>()))
                .ReturnsAsync(false);

            UserRecipeViewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<RecipeUserView>()))
                .Returns((RecipeUserView v) => Task.FromResult(v));

            UserRecipeViewRepositoryMock
                .Setup(r => r.CountAsync(It.IsAny<Expression<Func<RecipeUserView, bool>>>()))
                .ReturnsAsync(7);

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RecipeUserSave, bool>>>()))
                .ReturnsAsync(false);

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse
                {
                    Id = recipe.Id,
                    Ingredients = new List<RecipeIngredientResponse>()
                });

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    viewerId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new AppUser
                {
                    Id = viewerId,
                    DietRestrictions = new List<UserDietRestriction>()
                });

            var res = await Sut.GetRecipeDetailsAsync(viewerId, recipe.Id);

            Assert.Equal(7, recipe.ViewCount);
        }

        [Fact]
        public async Task Details_Owner_ShouldNotCreateView()
        {
            var ownerId = NewId();

            var recipe = new Recipe
            {
                Id = NewId(),
                AuthorId = ownerId,
                Status = RecipeStatus.Posted,
                RecipeIngredients = new List<RecipeIngredient>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipe.Id, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            SetupUser(ownerId);

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RecipeUserSave, bool>>>()))
                .ReturnsAsync(false);

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse
                {
                    Id = recipe.Id,
                    Ingredients = new List<RecipeIngredientResponse>()
                });

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    ownerId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new AppUser { Id = ownerId });

            var _ = await Sut.GetRecipeDetailsAsync(ownerId, recipe.Id);

            UserRecipeViewRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Details_ShouldSet_IsSaved()
        {
            var userId = NewId();
            var recipe = new Recipe
            {
                Id = NewId(),
                AuthorId = NewId(),
                Status = RecipeStatus.Posted,
                RecipeIngredients = new List<RecipeIngredient>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipe.Id, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            SetupUser(userId);

            UserRecipeViewRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RecipeUserView, bool>>>()))
                .ReturnsAsync(false);

            UserRecipeViewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<RecipeUserView>()))
                .Returns((RecipeUserView v) => Task.FromResult(v));

            UserRecipeViewRepositoryMock
                .Setup(r => r.CountAsync(It.IsAny<Expression<Func<RecipeUserView, bool>>>()))
                .ReturnsAsync(2);

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RecipeUserSave, bool>>>()))
                .ReturnsAsync(true);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new AppUser { Id = userId, DietRestrictions = new List<UserDietRestriction>() });

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse
                {
                    Id = recipe.Id,
                    Ingredients = new List<RecipeIngredientResponse>(),
                    IsSaved = false
                });

            var res = await Sut.GetRecipeDetailsAsync(userId, recipe.Id);

            Assert.True(res.IsSaved);
        }

        [Fact]
        public async Task Details_ShouldApply_IngredientRestriction()
        {
            var userId = NewId();
            var ingredientId = NewId();

            var recipe = new Recipe
            {
                Id = NewId(),
                AuthorId = NewId(),
                Status = RecipeStatus.Posted,
                RecipeIngredients =
                {
                    new RecipeIngredient
                    {
                        IngredientId = ingredientId,
                        Ingredient = new Ingredient
                        {
                            Id = ingredientId,
                            Categories = new List<IngredientCategory>()
                        }
                    }
                }
            };

            var user = new AppUser
            {
                Id = userId,
                DietRestrictions =
                {
                    new UserDietRestriction
                    {
                        IngredientId = ingredientId,
                        Type = RestrictionType.Allergy
                    }
                }
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipe.Id, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            UserRecipeViewRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RecipeUserView, bool>>>()))
                .ReturnsAsync(false);

            UserRecipeViewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<RecipeUserView>()))
                .Returns((RecipeUserView v) => Task.FromResult(v));

            UserRecipeViewRepositoryMock
                .Setup(r => r.CountAsync(It.IsAny<Expression<Func<RecipeUserView, bool>>>()))
                .ReturnsAsync(1);

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RecipeUserSave, bool>>>()))
                .ReturnsAsync(false);

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse
                {
                    Id = recipe.Id,
                    Ingredients =
                    {
                        new RecipeIngredientResponse { IngredientId = ingredientId }
                    }
                });

            var res = await Sut.GetRecipeDetailsAsync(userId, recipe.Id);

            Assert.Equal(RestrictionType.Allergy, res.Ingredients[0].RestrictionType);
        }

        [Fact]
        public async Task Details_ShouldApply_CategoryRestriction()
        {
            var userId = NewId();
            var ingredientId = NewId();
            var categoryId = NewId();

            var recipe = new Recipe
            {
                Id = NewId(),
                AuthorId = NewId(),
                Status = RecipeStatus.Posted,
                RecipeIngredients =
                {
                    new RecipeIngredient
                    {
                        IngredientId = ingredientId,
                        Ingredient = new Ingredient
                        {
                            Id = ingredientId,
                            Categories =
                            {
                                new IngredientCategory { Id = categoryId }
                            }
                        }
                    }
                }
            };

            var user = new AppUser
            {
                Id = userId,
                DietRestrictions =
                {
                    new UserDietRestriction
                    {
                        IngredientCategoryId = categoryId,
                        Type = RestrictionType.Dislike
                    }
                }
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipe.Id, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            UserRecipeViewRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RecipeUserView, bool>>>()))
                .ReturnsAsync(false);

            UserRecipeViewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<RecipeUserView>()))
                .Returns((RecipeUserView v) => Task.FromResult(v));

            UserRecipeViewRepositoryMock
                .Setup(r => r.CountAsync(It.IsAny<Expression<Func<RecipeUserView, bool>>>()))
                .ReturnsAsync(2);

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RecipeUserSave, bool>>>()))
                .ReturnsAsync(false);

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse
                {
                    Id = recipe.Id,
                    Ingredients =
                    {
                        new RecipeIngredientResponse { IngredientId = ingredientId }
                    }
                });

            var res = await Sut.GetRecipeDetailsAsync(userId, recipe.Id);

            Assert.Equal(RestrictionType.Dislike, res.Ingredients[0].RestrictionType);
        }
    }
}
