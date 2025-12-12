using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Specifications;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeQueryServiceTests
{
    public class GetRecipesAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task GetRecipes_ShouldThrow_WhenIncludeIngredientIds_NotExist()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            var req = new RecipeFilterRequest
            {
                IncludeIngredientIds = new List<Guid> { NewId() },
                PaginationParams = new RecipePaginationParams()
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.GetRecipesAsync(req));
            IngredientRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetRecipes_ShouldThrow_WhenExcludeIngredientIds_NotExist()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            var req = new RecipeFilterRequest
            {
                ExcludeIngredientIds = new List<Guid> { NewId() },
                PaginationParams = new RecipePaginationParams()
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.GetRecipesAsync(req));
            IngredientRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetRecipes_ShouldThrow_WhenIncludeLabelIds_NotExist()
        {
            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            var req = new RecipeFilterRequest
            {
                IncludeLabelIds = new List<Guid> { NewId() },
                PaginationParams = new RecipePaginationParams()
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.GetRecipesAsync(req));
            LabelRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetRecipes_ShouldThrow_WhenExcludeLabelIds_NotExist()
        {
            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            var req = new RecipeFilterRequest
            {
                ExcludeLabelIds = new List<Guid> { NewId() },
                PaginationParams = new RecipePaginationParams()
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.GetRecipesAsync(req));
            LabelRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetRecipes_ShouldThrow_WhenIngredient_IncludeAndExclude_Conflict()
        {
            var id = NewId();

            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            var req = new RecipeFilterRequest
            {
                IncludeIngredientIds = new List<Guid> { id },
                ExcludeIngredientIds = new List<Guid> { id },
                PaginationParams = new RecipePaginationParams()
            };

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.GetRecipesAsync(req));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);
        }

        [Fact]
        public async Task GetRecipes_ShouldThrow_WhenLabel_IncludeAndExclude_Conflict()
        {
            var id = NewId();

            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            var req = new RecipeFilterRequest
            {
                IncludeLabelIds = new List<Guid> { id },
                ExcludeLabelIds = new List<Guid> { id },
                PaginationParams = new RecipePaginationParams()
            };

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.GetRecipesAsync(req));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);
        }

        [Fact]
        public async Task GetRecipes_ShouldNotThrow_WhenIngredient_IncludeAndExclude_AreDifferent()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            RecipeRepositoryMock
                .Setup(r => r.GetRecipesRawAsync(It.IsAny<RecipeBasicFilterSpec>()))
                .ReturnsAsync(new List<Recipe>());

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeResponse>());

            var req = new RecipeFilterRequest
            {
                IncludeIngredientIds = new List<Guid> { NewId() },
                ExcludeIngredientIds = new List<Guid> { NewId() },
                PaginationParams = new RecipePaginationParams()
            };

            var res = await Sut.GetRecipesAsync(req);

            Assert.Empty(res.Items);
        }

        [Fact]
        public async Task GetRecipes_ShouldNotThrow_WhenLabel_IncludeAndExclude_AreDifferent()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            RecipeRepositoryMock
                .Setup(r => r.GetRecipesRawAsync(It.IsAny<RecipeBasicFilterSpec>()))
                .ReturnsAsync(new List<Recipe>());

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeResponse>());

            var req = new RecipeFilterRequest
            {
                IncludeLabelIds = new List<Guid> { NewId() },
                ExcludeLabelIds = new List<Guid> { NewId() },
                PaginationParams = new RecipePaginationParams()
            };

            var res = await Sut.GetRecipesAsync(req);

            Assert.Empty(res.Items);
        }


        [Fact]
        public async Task GetRecipes_ShouldFilter_ByIncludeIngredients()
        {
            var ing1 = NewId();
            var ing2 = NewId();

            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = NewId(),
                    Name = "A",
                    RecipeIngredients = new List<RecipeIngredient>
                    {
                        new() { IngredientId = ing1 }
                    }
                },
                new Recipe
                {
                    Id = NewId(),
                    Name = "B",
                    RecipeIngredients = new List<RecipeIngredient>
                    {
                        new() { IngredientId = ing2 }
                    }
                }
            };

            RecipeRepositoryMock
                .Setup(r => r.GetRecipesRawAsync(It.IsAny<RecipeBasicFilterSpec>()))
                .ReturnsAsync(recipes);

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeResponse>());

            var req = new RecipeFilterRequest
            {
                IncludeIngredientIds = new List<Guid> { ing1 },
                PaginationParams = new RecipePaginationParams()
            };

            var result = await Sut.GetRecipesAsync(req);

            Assert.Equal(1, result.TotalCount);
            IngredientRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task GetRecipes_ShouldSort_NameAsc()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            var r1 = new Recipe { Id = NewId(), Name = "B", RecipeIngredients = new List<RecipeIngredient>() };
            var r2 = new Recipe { Id = NewId(), Name = "A", RecipeIngredients = new List<RecipeIngredient>() };

            var recipes = new List<Recipe> { r1, r2 };

            RecipeRepositoryMock
                .Setup(r => r.GetRecipesRawAsync(It.IsAny<RecipeBasicFilterSpec>()))
                .ReturnsAsync(recipes);

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeResponse>
                {
                    new RecipeResponse { Id = r2.Id },
                    new RecipeResponse { Id = r1.Id }
                });

            var req = new RecipeFilterRequest
            {
                SortBy = "name_asc",
                PaginationParams = new RecipePaginationParams()
            };

            var res = await Sut.GetRecipesAsync(req);

            Assert.Equal(2, res.Items.Count());
            Assert.Equal(r2.Id, res.Items.ElementAt(0).Id);
            Assert.Equal(r1.Id, res.Items.ElementAt(1).Id);
        }

        [Fact]
        public async Task GetRecipes_ShouldApplyPagination()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            var recipes = Enumerable.Range(1, 20)
                .Select(i => new Recipe
                {
                    Id = Guid.NewGuid(),
                    Name = $"R{i}",
                    RecipeIngredients = new List<RecipeIngredient>()
                })
                .ToList();

            RecipeRepositoryMock
                .Setup(r => r.GetRecipesRawAsync(It.IsAny<RecipeBasicFilterSpec>()))
                .ReturnsAsync(recipes);

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns((List<Recipe> src) =>
                    src.Select(x => new RecipeResponse { Id = x.Id }).ToList());

            var req = new RecipeFilterRequest
            {
                PaginationParams = new RecipePaginationParams
                {
                    PageNumber = 2,
                }
            };

            var res = await Sut.GetRecipesAsync(req);

            Assert.Equal(8, res.Items.Count());
            Assert.Equal(20, res.TotalCount);
        }

        [Fact]
        public async Task GetRecipes_ShouldMapCorrectly()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            var recipe = new Recipe
            {
                Id = NewId(),
                Name = "Test",
                RecipeIngredients = new List<RecipeIngredient>()
            };

            RecipeRepositoryMock
                .Setup(r => r.GetRecipesRawAsync(It.IsAny<RecipeBasicFilterSpec>()))
                .ReturnsAsync(new List<Recipe> { recipe });

            MapperMock
                .Setup(m => m.Map<List<RecipeResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeResponse>
                {
                    new RecipeResponse { Id = recipe.Id }
                });

            var req = new RecipeFilterRequest
            {
                PaginationParams = new RecipePaginationParams()
            };

            var res = await Sut.GetRecipesAsync(req);

            Assert.Single(res.Items);
            Assert.Equal(recipe.Id, res.Items.ElementAt(0).Id);
        }
    }
}
