using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.RecipeQueryServiceTests
{
    public class GetRecipeDetailsAsyncTests : RecipeQueryServiceTestBase
    {
        private void SetupUser(Guid id, bool hasPermission = false)
        {
            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new AppUser
                {
                    Id = id,
                    Role = new AppRole
                    {
                        RolePermissions = hasPermission
                            ? new List<AppRolePermission>
                            {
                                new()
                                {
                                    PermissionAction = new PermissionAction
                                    {
                                        Name = PermissionValue.Recipe_ManagementView.Action,
                                        PermissionDomain = new PermissionDomain
                                        {
                                            Name = PermissionValue.Recipe_ManagementView.Domain
                                        }
                                    }
                                }
                            }
                            : new List<AppRolePermission>()
                    }
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
                Status = RecipeStatus.Pending,
                AuthorId = NewId()
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
        public async Task Details_ShouldThrow_WhenStatusNotPosted_AndUserNotOwner()
        {
            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Pending,
                AuthorId = NewId()
            };

            var callerId = NewId();

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            SetupUser(callerId);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeDetailsAsync(callerId, recipe.Id));
        }

        [Fact]
        public async Task Details_Guest_ShouldReturnMappedResult_AndNotAddView()
        {
            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                AuthorId = NewId(),
                RecipeIngredients = new List<RecipeIngredient>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse { Id = recipe.Id });

            var result = await Sut.GetRecipeDetailsAsync(null, recipe.Id);

            Assert.NotNull(result);
            Assert.Equal(recipe.Id, result.Id);

            UserRecipeViewRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Details_UserNotOwner_ShouldCreateView_AndUpdateViewCount()
        {
            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                AuthorId = NewId(),
                RecipeIngredients = new List<RecipeIngredient>()
            };

            var viewerId = NewId();

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            SetupUser(viewerId);

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserSaveRecipe, bool>>>()))
                .ReturnsAsync(false);

            UserRecipeViewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserRecipeView>()))
                .Returns((UserRecipeView r) => Task.FromResult(r));

            UserRecipeViewRepositoryMock
                .Setup(r => r.CountAsync(It.IsAny<Expression<Func<UserRecipeView, bool>>>()))
                .ReturnsAsync(5);

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse
                {
                    Id = recipe.Id,
                    Ingredients = new List<RecipeIngredientResponse>()
                });

            var res = await Sut.GetRecipeDetailsAsync(viewerId, recipe.Id);

            Assert.Equal(5, recipe.ViewCount);
        }

        [Fact]
        public async Task Details_Owner_ShouldNotCreateView()
        {
            var userId = NewId();

            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                AuthorId = userId,
                RecipeIngredients = new List<RecipeIngredient>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            SetupUser(userId);

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserSaveRecipe, bool>>>()))
                .ReturnsAsync(false);

            MapperMock
                .Setup(m => m.Map<RecipeDetailsResponse>(recipe))
                .Returns(new RecipeDetailsResponse
                {
                    Id = recipe.Id,
                    Ingredients = new List<RecipeIngredientResponse>()
                });

            var _ = await Sut.GetRecipeDetailsAsync(userId, recipe.Id);

            UserRecipeViewRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Details_ShouldSet_IsSaved()
        {
            var userId = NewId();

            var recipe = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Posted,
                AuthorId = NewId(),
                RecipeIngredients = new List<RecipeIngredient>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            SetupUser(userId);

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserSaveRecipe, bool>>>()))
                .ReturnsAsync(true);

            UserRecipeViewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserRecipeView>()))
                .Returns((UserRecipeView r) => Task.FromResult(r));

            UserRecipeViewRepositoryMock
                .Setup(r => r.CountAsync(It.IsAny<Expression<Func<UserRecipeView, bool>>>()))
                .ReturnsAsync(1);

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

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
                Status = RecipeStatus.Posted,
                AuthorId = NewId(),
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
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserSaveRecipe, bool>>>()))
                .ReturnsAsync(false);

            UserRecipeViewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserRecipeView>()))
                .Returns((UserRecipeView r) => Task.FromResult(r));

            UserRecipeViewRepositoryMock
                .Setup(r => r.CountAsync(It.IsAny<Expression<Func<UserRecipeView, bool>>>()))
                .ReturnsAsync(1);

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

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
                Status = RecipeStatus.Posted,
                AuthorId = NewId(),
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
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserSaveRecipe, bool>>>()))
                .ReturnsAsync(false);

            UserRecipeViewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserRecipeView>()))
                .Returns((UserRecipeView r) => Task.FromResult(r));

            UserRecipeViewRepositoryMock
                .Setup(r => r.CountAsync(It.IsAny<Expression<Func<UserRecipeView, bool>>>()))
                .ReturnsAsync(3);

            RecipeRepositoryMock
                .Setup(r => r.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

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
