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

            RecipeRepositoryMock.VerifyAll();
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

            RecipeRepositoryMock.VerifyAll();
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

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    recipe.Id,
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRecipeDetailsAsync(NewId(), recipe.Id));

            RecipeRepositoryMock.VerifyAll();
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

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
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

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    viewerId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new AppUser
                {
                    Id = viewerId,
                    DietRestrictions = new List<UserDietRestriction>()
                });

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserSaveRecipe, bool>>>()))
                .ReturnsAsync(false);

            UserRecipeViewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserRecipeView>()))
                .Returns((UserRecipeView urv) => Task.FromResult(urv));

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

            var result = await Sut.GetRecipeDetailsAsync(viewerId, recipe.Id);

            Assert.Equal(5, recipe.ViewCount);

            RecipeRepositoryMock.VerifyAll();
            UserRecipeViewRepositoryMock.VerifyAll();
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

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new AppUser
                {
                    Id = userId,
                    DietRestrictions = new List<UserDietRestriction>()
                });

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

            var result = await Sut.GetRecipeDetailsAsync(userId, recipe.Id);

            UserRecipeViewRepositoryMock.VerifyNoOtherCalls();
            RecipeRepositoryMock.VerifyAll();
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

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(new AppUser
                {
                    Id = userId,
                    DietRestrictions = new List<UserDietRestriction>()
                });

            UserSaveRecipeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserSaveRecipe, bool>>>()))
                .ReturnsAsync(true);

            UserRecipeViewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserRecipeView>()))
                .Returns((UserRecipeView urv) => Task.FromResult(urv));

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
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new()
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
                DietRestrictions = new List<UserDietRestriction>
                {
                    new()
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
                .Returns((UserRecipeView urv) => Task.FromResult(urv));

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
                    Ingredients = new List<RecipeIngredientResponse>
                    {
                        new()
                        {
                            IngredientId = ingredientId
                        }
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
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new()
                    {
                        IngredientId = ingredientId,
                        Ingredient = new Ingredient
                        {
                            Id = ingredientId,
                            Categories = new List<IngredientCategory>
                            {
                                new() { Id = categoryId }
                            }
                        }
                    }
                }
            };

            var user = new AppUser
            {
                Id = userId,
                DietRestrictions = new List<UserDietRestriction>
                {
                    new()
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
                .Returns((UserRecipeView urv) => Task.FromResult(urv));

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
                    Ingredients = new List<RecipeIngredientResponse>
                    {
                        new() { IngredientId = ingredientId }
                    }
                });

            var res = await Sut.GetRecipeDetailsAsync(userId, recipe.Id);

            Assert.Equal(RestrictionType.Dislike, res.Ingredients[0].RestrictionType);
        }
    }
}
