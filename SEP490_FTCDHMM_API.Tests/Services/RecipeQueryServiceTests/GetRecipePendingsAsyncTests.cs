using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.RecipeQueryServiceTests
{
    public class GetRecipePendingsAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task Pendings_ShouldReturnPagedResult()
        {
            var r1 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Pending,
                Name = "A",
                Author = new AppUser { Id = NewId() },
                Image = new Image { Id = NewId() }
            };

            var r2 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Pending,
                Name = "B",
                Author = new AppUser { Id = NewId() },
                Image = new Image { Id = NewId() }
            };

            var recipes = new List<Recipe> { r1, r2 };

            RecipeRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>()
                ))
                .ReturnsAsync((recipes, recipes.Count));

            MapperMock
                .Setup(m => m.Map<IReadOnlyList<RecipeManagementResponse>>(recipes))
                .Returns(recipes.Select(x => new RecipeManagementResponse { Id = x.Id }).ToList());

            var req = new PaginationParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await Sut.GetRecipePendingsAsync(req);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.Contains(result.Items, x => x.Id == r1.Id);
            Assert.Contains(result.Items, x => x.Id == r2.Id);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task Pendings_ShouldApplyPaginationCorrectly()
        {
            var list = Enumerable.Range(1, 12)
                .Select(i => new Recipe
                {
                    Id = Guid.NewGuid(),
                    Status = RecipeStatus.Pending,
                    Name = "R" + i,
                    Author = new AppUser { Id = Guid.NewGuid() },
                    Image = new Image { Id = Guid.NewGuid() }
                })
                .ToList();

            var pageItems = list.Skip(5).Take(5).ToList();

            RecipeRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>()
                ))
                .ReturnsAsync((pageItems, list.Count));

            MapperMock
                .Setup(m => m.Map<IReadOnlyList<RecipeManagementResponse>>(pageItems))
                .Returns(pageItems.Select(x => new RecipeManagementResponse { Id = x.Id }).ToList());

            var req = new PaginationParams
            {
                PageNumber = 2,
                PageSize = 5
            };

            var result = await Sut.GetRecipePendingsAsync(req);

            Assert.Equal(12, result.TotalCount);
            Assert.Equal(5, result.Items.Count());

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task Pendings_ShouldReturnEmpty_WhenNoPending()
        {
            RecipeRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>()
                ))
                .ReturnsAsync((new List<Recipe>(), 0));

            MapperMock
                .Setup(m => m.Map<IReadOnlyList<RecipeManagementResponse>>(It.IsAny<List<Recipe>>()))
                .Returns(new List<RecipeManagementResponse>());

            var req = new PaginationParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await Sut.GetRecipePendingsAsync(req);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
