using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserDietRestrictionServiceTests
{
    public class CreateIngredientRestrictionAsyncTests : UserDietRestrictionServiceTestBase
    {
        [Fact]
        public async Task CreateIngredientRestrictionAsync_ShouldThrow_WhenExpiredPast()
        {
            var dto = new CreateIngredientRestrictionRequest
            {
                IngredientId = Guid.NewGuid(),
                Type = RestrictionType.TemporaryAvoid.Value,
                ExpiredAtUtc = DateTime.UtcNow.AddDays(-1)
            };

            await Assert.ThrowsAsync<AppException>(() =>
                _service.CreateIngredientRestrictionAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task CreateIngredientRestrictionAsync_ShouldThrow_WhenIngredientNotFound()
        {
            _mockIngredientRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                               .ReturnsAsync(false);

            var dto = new CreateIngredientRestrictionRequest
            {
                IngredientId = Guid.NewGuid(),
                Type = RestrictionType.TemporaryAvoid.Value
            };

            await Assert.ThrowsAsync<AppException>(() =>
                _service.CreateIngredientRestrictionAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task CreateIngredientRestrictionAsync_ShouldThrow_WhenDuplicateExists()
        {
            _mockIngredientRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                               .ReturnsAsync(true);

            _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserDietRestriction, bool>>>()))
                     .ReturnsAsync(true);

            var dto = new CreateIngredientRestrictionRequest
            {
                IngredientId = Guid.NewGuid(),
                Type = RestrictionType.TemporaryAvoid.Value
            };

            await Assert.ThrowsAsync<AppException>(() =>
                _service.CreateIngredientRestrictionAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task CreateIngredientRestrictionAsync_ShouldCreateRestriction()
        {
            _mockIngredientRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                               .ReturnsAsync(true);

            _mockRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UserDietRestriction, bool>>>()))
                     .ReturnsAsync(false);

            var dto = new CreateIngredientRestrictionRequest
            {
                IngredientId = Guid.NewGuid(),
                Type = RestrictionType.Dislike.Value
            };

            await _service.CreateIngredientRestrictionAsync(Guid.NewGuid(), dto);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<UserDietRestriction>()), Times.Once);
        }
    }
}
