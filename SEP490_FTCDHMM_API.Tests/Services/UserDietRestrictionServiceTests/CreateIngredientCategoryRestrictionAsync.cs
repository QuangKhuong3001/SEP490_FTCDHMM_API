using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserDietRestrictionServiceTests
{
    public class CreateIngredientCategoryRestrictionAsyncTests : UserDietRestrictionServiceTestBase
    {
        [Fact]
        public async Task CreateIngredientCategoryRestrictionAsync_ShouldThrow_WhenExpiredAt_IsPast()
        {
            var dto = new CreateIngredientCategoryRestrictionRequest
            {
                IngredientCategoryId = Guid.NewGuid(),
                Type = RestrictionType.TemporaryAvoid.Value,
                ExpiredAtUtc = DateTime.UtcNow.AddDays(-1)
            };

            await Assert.ThrowsAsync<AppException>(() =>
                _service.CreateIngredientCategoryRestrictionAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task CreateIngredientCategoryRestrictionAsync_ShouldThrow_WhenCategoryNotFound()
        {
            _mockCategoryRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<IngredientCategory, bool>>>()))
                             .ReturnsAsync(false);

            var dto = new CreateIngredientCategoryRestrictionRequest
            {
                IngredientCategoryId = Guid.NewGuid(),
                Type = RestrictionType.TemporaryAvoid.Value
            };

            await Assert.ThrowsAsync<AppException>(() =>
                _service.CreateIngredientCategoryRestrictionAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task CreateIngredientCategoryRestrictionAsync_ShouldThrow_WhenDuplicateExists()
        {
            _mockCategoryRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<IngredientCategory, bool>>>()))
                             .ReturnsAsync(true);

            _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserDietRestriction, bool>>>()))
                     .ReturnsAsync(true);

            var dto = new CreateIngredientCategoryRestrictionRequest
            {
                IngredientCategoryId = Guid.NewGuid(),
                Type = RestrictionType.TemporaryAvoid.Value
            };

            await Assert.ThrowsAsync<AppException>(() =>
                _service.CreateIngredientCategoryRestrictionAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task CreateIngredientCategoryRestrictionAsync_ShouldCreateRestriction()
        {
            _mockCategoryRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<IngredientCategory, bool>>>()))
                             .ReturnsAsync(true);
            _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserDietRestriction, bool>>>()))
                     .ReturnsAsync(false);

            var dto = new CreateIngredientCategoryRestrictionRequest
            {
                IngredientCategoryId = Guid.NewGuid(),
                Type = RestrictionType.Dislike.Value,
                ExpiredAtUtc = null
            };

            await _service.CreateIngredientCategoryRestrictionAsync(Guid.NewGuid(), dto);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<UserDietRestriction>()), Times.Once);
        }
    }
}
