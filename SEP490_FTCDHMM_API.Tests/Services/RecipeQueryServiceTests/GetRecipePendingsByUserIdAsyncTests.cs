using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeQueryServiceTests
{
    public class GetRecipePendingsByUserIdAsyncTests : RecipeQueryServiceTestBase
    {
        [Fact]
        public async Task PendingsByUser_ShouldReturnPagedResult()
        {
            var userId = NewId();

            var r1 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Pending,
                AuthorId = userId,
                Name = "A",
                Author = new AppUser { Id = userId },
                Image = new Image { Id = NewId() }
            };

            var r2 = new Recipe
            {
                Id = NewId(),
                Status = RecipeStatus.Locked,
                AuthorId = userId,
                Name = "B",
                Author = new AppUser { Id = userId },
                Image = new Image { Id = NewId() }
            };

            var list = new List<Recipe> { r1, r2 };

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
                .ReturnsAsync((list, list.Count));

            MapperMock
                .Setup(m => m.Map<IReadOnlyList<RecipeManagementResponse>>(list))
                .Returns(list.Select(x => new RecipeManagementResponse { Id = x.Id }).ToList());

            var req = new PaginationParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await Sut.GetRecipePendingsByUserIdAsync(userId, req);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.Contains(result.Items, x => x.Id == r1.Id);
            Assert.Contains(result.Items, x => x.Id == r2.Id);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task PendingsByUser_ShouldApplyPaginationCorrectly()
        {
            var userId = NewId();

            var all = Enumerable.Range(1, 15)
                .Select(i => new Recipe
                {
                    Id = Guid.NewGuid(),
                    Status = i % 2 == 0 ? RecipeStatus.Pending : RecipeStatus.Locked,
                    AuthorId = userId,
                    Name = "R" + i,
                    Author = new AppUser { Id = userId },
                    Image = new Image { Id = Guid.NewGuid() }
                })
                .ToList();

            var pageItems = all.Skip(5).Take(5).ToList();

            RecipeRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    2,
                    5,
                    It.IsAny<Expression<Func<Recipe, bool>>>(),
                    It.IsAny<Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>?>()
                ))
                .ReturnsAsync((pageItems, all.Count));

            MapperMock
                .Setup(m => m.Map<IReadOnlyList<RecipeManagementResponse>>(pageItems))
                .Returns(pageItems.Select(x => new RecipeManagementResponse { Id = x.Id }).ToList());

            var req = new PaginationParams
            {
                PageNumber = 2,
                PageSize = 5
            };

            var result = await Sut.GetRecipePendingsByUserIdAsync(userId, req);

            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Items.Count());

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }

        [Fact]
        public async Task PendingsByUser_ShouldReturnEmpty_WhenNoPendingOrLocked()
        {
            var userId = NewId();

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

            var result = await Sut.GetRecipePendingsByUserIdAsync(userId, req);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);

            RecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
