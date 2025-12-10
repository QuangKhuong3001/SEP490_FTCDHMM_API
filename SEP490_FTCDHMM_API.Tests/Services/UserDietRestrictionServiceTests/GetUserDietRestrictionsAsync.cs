using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.UserDietRestrictionServiceTests
{
    public class GetUserDietRestrictionsAsyncTests : UserDietRestrictionServiceTestBase
    {
        [Fact]
        public async Task GetUserDietRestrictionsAsync_ShouldFilterByKeyword()
        {
            var userId = Guid.NewGuid();

            var restrictions = new List<UserDietRestriction>
            {
                new() { Ingredient = new Ingredient { Name = "Cá Hồi" }, UserId = userId },
                new() { Ingredient = new Ingredient { Name = "Thịt Bò" }, UserId = userId }
            };

            _mockRepo.Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<UserDietRestriction, bool>>>(),
                    It.IsAny<Func<IQueryable<UserDietRestriction>, IQueryable<UserDietRestriction>>>()
                ))
                .ReturnsAsync(restrictions);

            var request = new UserDietRestrictionFilterRequest
            {
                Keyword = "ca hoi"
            };

            var result = await _service.GetUserDietRestrictionsAsync(userId, request);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetUserDietRestrictionsAsync_ShouldFilterByType()
        {
            var userId = Guid.NewGuid();

            var restrictions = new List<UserDietRestriction>
            {
                new() { Type = RestrictionType.Dislike, UserId = userId },
                new() { Type = RestrictionType.TemporaryAvoid, UserId = userId }
            };

            _mockRepo.Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<UserDietRestriction, bool>>>(),
                    It.IsAny<Func<IQueryable<UserDietRestriction>, IQueryable<UserDietRestriction>>>()
                ))
                .ReturnsAsync(restrictions);

            var request = new UserDietRestrictionFilterRequest
            {
                Type = RestrictionType.TemporaryAvoid.Value
            };

            var result = await _service.GetUserDietRestrictionsAsync(userId, request);

            Assert.Single(result);
            Assert.Equal(RestrictionType.TemporaryAvoid, result.First().Type);
        }

        [Fact]
        public async Task GetUserDietRestrictionsAsync_ShouldSortByNameAsc()
        {
            var userId = Guid.NewGuid();

            var restrictions = new List<UserDietRestriction>
            {
                new() { Ingredient = new Ingredient { Name = "B" }, UserId = userId },
                new() { Ingredient = new Ingredient { Name = "A" }, UserId = userId }
            };

            _mockRepo.Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<UserDietRestriction, bool>>>(),
                    It.IsAny<Func<IQueryable<UserDietRestriction>, IQueryable<UserDietRestriction>>>()
                ))
                .ReturnsAsync(restrictions);

            var request = new UserDietRestrictionFilterRequest
            {
                SortBy = "name_asc"
            };

            var result = await _service.GetUserDietRestrictionsAsync(userId, request);

            Assert.Equal("A", result.First().IngredientName);
        }
    }
}
