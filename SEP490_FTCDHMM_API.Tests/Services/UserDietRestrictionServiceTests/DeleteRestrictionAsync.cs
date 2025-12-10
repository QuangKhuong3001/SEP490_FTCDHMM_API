using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserDietRestrictionServiceTests
{
    public class DeleteRestrictionAsyncTests : UserDietRestrictionServiceTestBase
    {
        [Fact]
        public async Task DeleteRestrictionAsync_ShouldThrow_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                     .ReturnsAsync((UserDietRestriction?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                _service.DeleteRestrictionAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteRestrictionAsync_ShouldThrow_WhenUserMismatch()
        {
            var restriction = new UserDietRestriction
            {
                UserId = Guid.NewGuid()
            };

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                     .ReturnsAsync(restriction);

            await Assert.ThrowsAsync<AppException>(() =>
                _service.DeleteRestrictionAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteRestrictionAsync_ShouldDelete_WhenValid()
        {
            var userId = Guid.NewGuid();

            var restriction = new UserDietRestriction
            {
                UserId = userId
            };

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                     .ReturnsAsync(restriction);

            await _service.DeleteRestrictionAsync(userId, Guid.NewGuid());

            _mockRepo.Verify(r => r.DeleteAsync(restriction), Times.Once);
        }
    }
}
